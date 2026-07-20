namespace Sdl.Community.AntidoteVerifier
{
    public interface IEditorService
    {
        int GetDocumentNoOfSegments();
        int GetActiveSegmentId();
        string GetSegmentText(int index);
        bool CanReplace(int index, int startPosition, int endPosition, string origString, string displayLanguage, ref string message, ref string explication);
        string GetDocumentName();
        string GetDocumentPath();
        void ReplaceTextInSegment(int index, int startPosition, int endPosition, string segmentText);
        void SelectText(int index, int startPosition, int endPosition);
        void ActivateDocument();
    }
}