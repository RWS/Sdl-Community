namespace SDLCommunityCleanUpTasks.Models
{
    public class ConversionFile
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public override string ToString()
        {
            return FileName;
        }
    }
}