namespace Sdl.Community.AntidoteVerifier
{
    public interface IEditorService
    {
        long GetDocumentId();

        long GetDocumentNoOfSegments();

        long GetCurrentSegmentId(long segmentId);
        long GetActiveSegmentId();

        string GetSegmentText(long segmentId);
    }
}