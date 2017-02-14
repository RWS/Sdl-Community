namespace Sdl.Community.Structures.Rates.Helpers
{
    public class DocumentAnalysisMerged
    {
        public bool TranslationChangesAdded { get; set; }
        public bool StatusChangeAdded { get; set; }
        public bool CommentChangeAdded { get; set; }

        public DocumentAnalysisMerged()
        {
            TranslationChangesAdded = false;
            StatusChangeAdded = false;
            CommentChangeAdded = false;
        }
    }
}
