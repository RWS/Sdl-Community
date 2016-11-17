namespace Sdl.Community.AntidoteVerifier
{
    public interface IEditorService
    {
        int GetDocumentId();

        int GetDocumentNoOfSegments();

        int GetCurrentSegmentId(int index);
        int GetActiveSegmentId();

        string GetSegmentText(int index);
        string GetSelection();

        bool CanReplace(int index, int startPosition, int endPosition, string origString, string displayLanguage, ref string message, ref string explication);
        string GetDocumentName();
        void ReplaceTextInSegment(int index, int startPosition, int endPosition, string segmentText);
        void SelectText(int index, int startPosition, int endPosition);
        void ActivateDocument();
    }
}