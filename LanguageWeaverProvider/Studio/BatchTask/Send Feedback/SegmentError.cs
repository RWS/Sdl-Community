namespace LanguageWeaverProvider.Studio.BatchTask.Send_Feedback
{
    public class SegmentError
    {
        public string Error { get; set; }
        public string Id { get; set; }
        public string SourceSegment { get; set; }
        public string Provider { get; set; }
    }
}