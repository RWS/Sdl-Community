using System;
using System.Collections.Generic;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Maps the Connectix protocol onto the Studio editor via <see cref="IEditorService"/>.
    /// Each correctable Studio segment is exposed as one Antidote text zone whose <c>zoneId</c> is
    /// globally unique across the documents launched over the connection (see
    /// <see cref="DocumentRegistry"/>). Because Antidote echoes the zone id back in <c>select</c> /
    /// <c>allowEdit</c> / <c>replace</c>, those callbacks are ROUTED to the document they belong to
    /// even when a different document was launched more recently (verified live: with index-only
    /// zone ids, a correction applied in Antidote's tab A executed against document B). Callbacks
    /// that carry no zone id (<c>getTextZones</c>, <c>docIsAvailable</c>, <c>documentPath</c>) can
    /// NOT be routed — the protocol gives them no document identifier — and are answered for the
    /// active (last launched) document; see the migration doc for the consequences and the
    /// tested-and-ruled-out workarounds (e.g. answering <c>docIsAvailable</c> false on a switch).
    /// Editor access is funnelled through <see cref="_runOnUi"/> because Studio editor objects are
    /// UI-thread affine while Connectix frames arrive on a background thread.
    /// </summary>
    public sealed class StudioWordProcessorAgent : IWordProcessorAgent
    {
        private const string PlainTextMarkup = "text";
        private const string CarriageReturn = "\n";

        private readonly Action<Action> _runOnUi;
        private readonly DocumentRegistry _documents = new DocumentRegistry();

        public StudioWordProcessorAgent(IEditorService editor, Action<Action> runOnUi = null)
        {
            if (editor == null) throw new ArgumentNullException(nameof(editor));
            _runOnUi = runOnUi ?? (action => action());
            SetActiveEditor(editor);
        }

        /// <summary>
        /// Points the agent at the currently active Studio document for the next launch. Called
        /// before a Corrector/Dictionaries/Guides launch so the persistent session reports the right
        /// documentPath and zones for the document the user launched from. The document is
        /// registered (keyed by its path, the same identity Antidote uses for the correction tab) so
        /// later zone-id-bearing callbacks can be routed back to it even after another document is
        /// launched.
        /// </summary>
        public void SetActiveEditor(IEditorService editor)
        {
            if (editor == null) throw new ArgumentNullException(nameof(editor));

            var path = OnUi(editor.GetDocumentPath);
            _documents.Register(editor, path);
        }

        public WordProcessorConfiguration Configuration()
        {
            var editor = _documents.Active.Editor;
            return new WordProcessorConfiguration
            {
                DocumentTitle = OnUi(() => editor.GetDocumentName()),
                ActiveMarkup = PlainTextMarkup,
                CarriageReturn = CarriageReturn,
                // Identify the correction by document path. Antidote keys the correction TAB by this
                // path (cacheIdType "forcePath") within one connection: launching a new path opens a
                // new tab, relaunching an existing path re-focuses its tab. It also persists/restores
                // the correction-state cache for the document across launches.
                CacheIdType = CacheIdentifierType.ForcePath,
                // Antidote applies minimal positional replacements directly (as the old COM
                // RemplaceIntervalle did), so the editor does not require a live selection.
                ReplaceWithoutSelection = true
            };
        }

        // Paired with cacheIdType "forcePath": the document's local file path. Antidote uses it as
        // the correction TAB identity (and the persisted correction-state cache key) across launches.
        public string DocumentPath()
        {
            var editor = _documents.Active.Editor;
            return OnUi(() => editor.GetDocumentPath());
        }

        // Always truthful. Answering false to announce a document switch was tried and ruled out
        // live (2026-07-11): Antidote re-inits but never re-asks documentPath, and a sustained false
        // strands the launch in a "(closed)" tab -- see the migration doc.
        public bool IsDocumentAvailable() => true;

        public IReadOnlyList<TextZone> ZonesToCorrect(bool forActiveSelection)
        {
            var active = _documents.Active;
            var editor = active.Editor;
            return OnUi(() =>
            {
                var count = editor.GetDocumentNoOfSegments();
                if (count <= 0)
                    return (IReadOnlyList<TextZone>)new List<TextZone>();

                var activeIndex = editor.GetActiveSegmentId();

                if (forActiveSelection)
                {
                    // Dictionaries / guides: a single focused zone for the active segment.
                    var index = activeIndex >= 1 && activeIndex <= count ? activeIndex : 1;
                    return new List<TextZone> { BuildZone(active, editor, index, true) };
                }

                // Corrector: every correctable segment, focusing the active one. Read in one call
                // so the editor walks the document once -- Antidote re-requests all zones on
                // window moves and scrolling, so this runs far more often than once per launch.
                var texts = editor.GetSegmentTexts();
                var zones = new List<TextZone>(texts.Count);
                for (var index = 1; index <= texts.Count; index++)
                {
                    zones.Add(new TextZone
                    {
                        Text = texts[index - 1],
                        ZoneId = active.MakeZoneId(index),
                        ZoneIsFocused = index == activeIndex
                    });
                }

                return zones;
            });
        }

        public bool AllowEdit(AllowEditParams parameters)
        {
            if (parameters == null || !_documents.TryResolveZone(parameters.ZoneId, out var editor, out var index))
                return false;

            return OnUi(() =>
            {
                var text = editor.GetSegmentText(index);
                if (text == null) return false;
                if (parameters.PositionStart < 0 ||
                    parameters.PositionEnd > text.Length ||
                    parameters.PositionEnd < parameters.PositionStart)
                    return false;

                var actual = text.Substring(parameters.PositionStart, parameters.PositionEnd - parameters.PositionStart);
                return actual == parameters.Context;
            });
        }

        public bool Replace(ReplaceParams parameters)
        {
            if (parameters == null || !_documents.TryResolveZone(parameters.ZoneId, out var editor, out var index))
                return false;

            return OnUi(() =>
            {
                var text = editor.GetSegmentText(index);
                if (text == null) return false;
                if (parameters.PositionStartReplace < 0 ||
                    parameters.PositionReplaceEnd > text.Length ||
                    parameters.PositionReplaceEnd < parameters.PositionStartReplace)
                    return false;

                // Preserve the COM lock checks (read-only segment / locked range) before replacing.
                var origString = text.Substring(
                    parameters.PositionStartReplace,
                    parameters.PositionReplaceEnd - parameters.PositionStartReplace);
                var message = string.Empty;
                var explication = string.Empty;
                if (!editor.CanReplace(index, parameters.PositionStartReplace, parameters.PositionReplaceEnd,
                        origString, string.Empty, ref message, ref explication))
                    return false;

                editor.ReplaceTextInSegment(index, parameters.PositionStartReplace, parameters.PositionReplaceEnd,
                    parameters.NewString);
                return true;
            });
        }

        public void Select(SelectParams parameters)
        {
            if (parameters == null || !_documents.TryResolveZone(parameters.ZoneId, out var editor, out var index))
                return;

            OnUi(() => editor.SelectText(index, parameters.PositionStart, parameters.PositionEnd));
        }

        public void ReturnToDocument()
        {
            var editor = _documents.Active.Editor;
            OnUi(() => editor.ActivateDocument());
        }

        private static TextZone BuildZone(DocumentRegistry.DocumentEntry document, IEditorService editor,
            int index, bool focused)
        {
            return new TextZone
            {
                Text = editor.GetSegmentText(index),
                ZoneId = document.MakeZoneId(index),
                ZoneIsFocused = focused
            };
        }

        private T OnUi<T>(Func<T> func)
        {
            var result = default(T);
            _runOnUi(() => result = func());
            return result;
        }

        private void OnUi(Action action) => _runOnUi(action);
    }
}
