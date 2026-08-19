using Sdl.Community.AntidoteVerifier.Extensions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.AntidoteVerifier
{
    /// <summary>
    /// Exposes one Studio document to the Antidote agent as a list of 1-based, non-empty target
    /// segments. A fresh instance is bound to the active document on every launch. Unknown segment
    /// indexes throw (dictionary indexer) — the Connectix layer relies on that to reject callbacks
    /// for zones this document does not contain.
    /// Segment pairs are resolved from the document on demand and never held between calls, so a
    /// correction is never applied to a stale copy of a segment the user edited in the meantime.
    /// </summary>
    public class EditorService : IEditorService
    {
        private readonly IStudioDocument _document;

        // 1-based Antidote zone index -> stable Studio identity of the segment.
        private readonly Dictionary<int, SegmentIdentity> _segmentsByIndex =
            new Dictionary<int, SegmentIdentity>();

        public EditorService(IStudioDocument document)
        {
            _document = document;
            IndexCorrectableSegments();
        }

        public void ActivateDocument()
        {
            ApplicationContext.EditorController.Activate(_document);
        }

        public bool CanReplace(int segmentId, int startPosition, int endPosition, string origString,
            string displayLanguage, ref string message, ref string explication)
        {
            var segmentPair = FindSegmentPair(segmentId);
            return segmentPair != null &&
                   segmentPair.Target.CanReplace(startPosition, endPosition, origString, displayLanguage,
                       ref message, ref explication);
        }

        public int GetActiveSegmentId()
        {
            // The editor has no active pair while Antidote's own window holds focus — which is exactly
            // when zone refreshes arrive. Throwing here would abort the whole getTextZones handler, and
            // ConnectixAgent answers a failed callback with NOTHING, leaving the corrector waiting.
            var activePair = _document.ActiveSegmentPair;
            if (activePair == null)
                return 1;

            var activeIdentity = IdentityOf(activePair);

            foreach (var entry in _segmentsByIndex)
            {
                if (entry.Value.Equals(activeIdentity))
                    return entry.Key;
            }

            return 1;
        }

        public string GetDocumentName()
        {
            return _document.ActiveFile.Name;
        }

        public string GetDocumentPath()
        {
            // Stable identity for Antidote's correction cache (cacheIdType "forcePath"): the local
            // file path uniquely and consistently identifies the document across launches. This
            // restores the parity the old COM plugin had via DonneIdDocumentCourant/
            // DocumentIdGenerator. Fall back to the display name for unsaved/virtual files that
            // have no local path.
            var localPath = _document.ActiveFile?.LocalFilePath;
            return string.IsNullOrEmpty(localPath) ? _document.ActiveFile?.Name : localPath;
        }

        public int GetDocumentNoOfSegments()
        {
            return _segmentsByIndex.Count;
        }

        public string GetSegmentText(int index)
        {
            // An index this document never had still throws (see FindSegmentPair). A known index whose
            // pair no longer resolves returns null instead, which the correction callbacks already read
            // as a plain "no" — better than failing the frame and answering nothing at all.
            return FindSegmentPair(index)?.Target.GetString();
        }

        /// <summary>
        /// Text of every correctable segment, in zone-index order. Walks the document ONCE.
        /// Resolving each segment separately (a full scan of the document's segment pairs per
        /// segment, with a GetParagraphUnitProperties() call per comparison) made a single zone
        /// refresh cost O(segments²) — and Antidote asks for every zone again whenever its window
        /// moves or the editor scrolls, which froze medium documents for minutes.
        /// </summary>
        public IReadOnlyList<string> GetSegmentTexts()
        {
            var segmentPairsByIdentity = MapSegmentPairs();
            var texts = new string[_segmentsByIndex.Count];

            for (var index = 1; index <= texts.Length; index++)
            {
                if (_segmentsByIndex.TryGetValue(index, out var identity) &&
                    segmentPairsByIdentity.TryGetValue(identity, out var segmentPair))
                    texts[index - 1] = segmentPair.Target.GetString();
            }

            return texts;
        }

        public void ReplaceTextInSegment(int segmentId, int startPosition, int endPosition, string replacementText)
        {
            var segmentPair = FindSegmentPair(segmentId);
            if (segmentPair == null)
                return;

            segmentPair.Target.Replace(startPosition, endPosition, replacementText);
            _document.UpdateSegmentPair(segmentPair);
        }

        public void SelectText(int index, int startPosition, int endPosition)
        {
            var segmentPair = FindSegmentPair(index);
            if (segmentPair == null)
                return;

            var paragraphUnitId = segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;

            _document.SetActiveSegmentPair(paragraphUnitId, segmentPair.Properties.Id.Id);
        }

        // Single-segment resolution for the correction callbacks (allowEdit / replace / select).
        // Those arrive a few at a time, so one scan each is fine; the whole-document path is
        // GetSegmentTexts.
        private ISegmentPair FindSegmentPair(int index)
        {
            // Throws KeyNotFoundException for an index outside this document — by design; see class docs.
            var identity = _segmentsByIndex[index];

            return _document.SegmentPairs.FirstOrDefault(segmentPair => IdentityOf(segmentPair).Equals(identity));
        }

        // One pass over the document's segment pairs, keyed by identity. Keeps the FIRST pair for a
        // given identity, matching the single-lookup FirstOrDefault behaviour.
        private Dictionary<SegmentIdentity, ISegmentPair> MapSegmentPairs()
        {
            var map = new Dictionary<SegmentIdentity, ISegmentPair>();
            foreach (var segmentPair in _document.SegmentPairs)
            {
                var identity = IdentityOf(segmentPair);
                if (!map.ContainsKey(identity))
                    map.Add(identity, segmentPair);
            }

            return map;
        }

        private void IndexCorrectableSegments()
        {
            _segmentsByIndex.Clear();
            if (_document == null)
                return;

            var index = 1;
            foreach (var segmentPair in _document.FilteredSegmentPairs)
            {
                if (string.IsNullOrEmpty(segmentPair.Target.GetString()))
                    continue;

                _segmentsByIndex.Add(index++, IdentityOf(segmentPair));
            }
        }

        private static SegmentIdentity IdentityOf(ISegmentPair segmentPair)
        {
            return new SegmentIdentity(
                segmentPair.Properties.Id.Id,
                segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id);
        }

        /// <summary>Studio's stable identity for one segment: its id within its paragraph unit.</summary>
        private readonly struct SegmentIdentity : IEquatable<SegmentIdentity>
        {
            public SegmentIdentity(string segmentId, string paragraphUnitId)
            {
                SegmentId = segmentId;
                ParagraphUnitId = paragraphUnitId;
            }

            public string SegmentId { get; }
            public string ParagraphUnitId { get; }

            public bool Equals(SegmentIdentity other)
                => string.Equals(SegmentId, other.SegmentId, StringComparison.Ordinal)
                   && string.Equals(ParagraphUnitId, other.ParagraphUnitId, StringComparison.Ordinal);

            public override bool Equals(object obj) => obj is SegmentIdentity other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((SegmentId?.GetHashCode() ?? 0) * 397) ^ (ParagraphUnitId?.GetHashCode() ?? 0);
                }
            }
        }
    }
}
