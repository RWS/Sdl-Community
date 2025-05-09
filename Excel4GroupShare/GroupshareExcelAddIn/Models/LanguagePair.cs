using Sdl.Community.GroupShareKit.Models.Response;

namespace GroupshareExcelAddIn.Models
{
    public class LanguagePair
    {
        public LanguagePair(Language sourceLanguage, Language targetLanguage)
        {
            SourceLanguage = sourceLanguage;
            TargetLanguage = targetLanguage;
        }

        public Language SourceLanguage { get; }
        public Language TargetLanguage { get; }
    }
}