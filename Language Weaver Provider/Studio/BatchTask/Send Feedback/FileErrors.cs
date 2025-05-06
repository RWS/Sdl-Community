using System.Collections.Generic;

namespace LanguageWeaverProvider.Studio.BatchTask.Send_Feedback
{
    public class FileErrors
    {
        public List<SegmentError> SegmentErrors { get; set; }
        public string Filename { get; set; }
    }
}