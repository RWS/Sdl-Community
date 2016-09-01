namespace Sdl.Community.AntidoteVerifier
{
    public interface IEditorService
    {
        int GetDocumentId();

        int GetDocumentNoOfSegments();

        int GetCurrentSegmentId(int segmentId);
        int GetActiveSegmentId();

        string GetSegmentText(int segmentId);
        string GetSelection();

        string GetDocumentName();
        void ReplaceTextInSegment(int segmentId, int startPosition, int endPosition, string segmentText);
        void SelectText(int segmentId, int startPosition, int endPosition);
        void ActivateDocument();
    }
}