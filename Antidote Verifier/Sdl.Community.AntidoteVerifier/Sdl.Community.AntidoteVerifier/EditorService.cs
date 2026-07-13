using Sdl.Community.AntidoteVerifier.Extensions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.AntidoteVerifier
{
    /// <summary>
    /// Exposes one Studio document to the Antidote agent as a list of 1-based, non-empty target
    /// segments. A fresh instance is bound to the active document on every launch. Unknown segment
    /// indexes throw (dictionary indexer) — the Connectix layer relies on that to reject callbacks
    /// for zones this document does not contain.
    /// </summary>
    public class EditorService : IEditorService
    {
        private readonly IStudioDocument _document;

        // 1-based Antidote zone index -> stable Studio identity of the segment. Segment pairs are
        // re-resolved from the document on every access because UpdateSegmentPair can replace the
        // underlying instances while a correction is running.
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
            var active = _document.ActiveSegmentPair;
            var segmentId = active.Properties.Id.Id;
            var paragraphUnitId = active.GetParagraphUnitProperties().ParagraphUnitId.Id;

            foreach (var entry in _segmentsByIndex)
            {
                if (entry.Value.Matches(segmentId, paragraphUnitId))
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
            return FindSegmentPair(index).Target.GetString();
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
            var paragraphUnitId = segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;

            _document.SetActiveSegmentPair(paragraphUnitId, segmentPair.Properties.Id.Id);
        }

        private ISegmentPair FindSegmentPair(int index)
        {
            // Throws KeyNotFoundException for an index outside this document — by design; see class docs.
            var identity = _segmentsByIndex[index];

            return _document.SegmentPairs.FirstOrDefault(segmentPair =>
                identity.Matches(
                    segmentPair.Properties.Id.Id,
                    segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id));
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

                _segmentsByIndex.Add(index++, new SegmentIdentity(
                    segmentPair.Properties.Id.Id,
                    segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id));
            }
        }

        /// <summary>Studio's stable identity for one segment: its id within its paragraph unit.</summary>
        private readonly struct SegmentIdentity
        {
            public SegmentIdentity(string segmentId, string paragraphUnitId)
            {
                SegmentId = segmentId;
                ParagraphUnitId = paragraphUnitId;
            }

            public string SegmentId { get; }
            public string ParagraphUnitId { get; }

            public bool Matches(string segmentId, string paragraphUnitId)
                => SegmentId.Equals(segmentId) && ParagraphUnitId.Equals(paragraphUnitId);
        }
    }
}
