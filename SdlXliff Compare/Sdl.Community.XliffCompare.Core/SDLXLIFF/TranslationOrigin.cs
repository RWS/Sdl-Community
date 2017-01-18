namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    internal class TranslationOrigin
    {
        internal bool IsRepeated { get; set; }
        internal bool IsStructureContextMatch { get; set; }

        internal int MatchPercentage { get; set; }

        internal string OriginSystem { get; set; }
        internal string OriginType { get; set; }

        internal string RepetitionTableId { get; set; }
        internal string TextContextMatchLevel { get; set; }

        internal TranslationOrigin()
        {
            IsRepeated = false;
            IsStructureContextMatch = false;

            MatchPercentage = 0;

            OriginSystem = "";
            OriginType = "";

            RepetitionTableId = "";
            TextContextMatchLevel = "";
        }

    }
}
