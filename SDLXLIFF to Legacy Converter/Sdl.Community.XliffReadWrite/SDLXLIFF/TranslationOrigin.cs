namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{
    public class TranslationOrigin
    {
        public bool IsRepeated { get; set; }
        public bool IsStructureContextMatch { get; set; }

        public int MatchPercentage { get; set; }

        public string OriginSystem { get; set; }
        public string OriginType { get; set; }

        public string RepetitionTableId { get; set; }
        public string TextContextMatchLevel { get; set; }

        public string OriginalTranslationHash { get; set; }

        public string OriginalTranslationHashSource { get; set; }
        public string OriginalTranslationHashTarget { get; set; }


        public TranslationOrigin()
        {
            IsRepeated = false;
            IsStructureContextMatch = false;

            MatchPercentage = 0;

            OriginSystem = string.Empty;
            OriginType = string.Empty;

            RepetitionTableId = string.Empty;
            TextContextMatchLevel = string.Empty;

            OriginalTranslationHash = string.Empty;
            OriginalTranslationHashSource = string.Empty;
            OriginalTranslationHashTarget = string.Empty;
        }


    }
}
