using System.Collections.Generic;

namespace Sdl.Community.AntidoteVerifier
{
    public interface IEditorService
    {
        int GetDocumentNoOfSegments();
        int GetActiveSegmentId();
        string GetSegmentText(int index);

        /// <summary>
        /// Text of every correctable segment, in zone-index order (zone 1 is the first element).
        /// Reading them in one call lets the implementation walk the document once instead of
        /// once per segment, which matters because Antidote re-requests every zone whenever its
        /// window moves or the editor scrolls.
        /// </summary>
        IReadOnlyList<string> GetSegmentTexts();
        bool CanReplace(int index, int startPosition, int endPosition, string origString, string displayLanguage, ref string message, ref string explication);
        string GetDocumentName();
        string GetDocumentPath();
        void ReplaceTextInSegment(int index, int startPosition, int endPosition, string segmentText);
        void SelectText(int index, int startPosition, int endPosition);
        void ActivateDocument();
    }
}