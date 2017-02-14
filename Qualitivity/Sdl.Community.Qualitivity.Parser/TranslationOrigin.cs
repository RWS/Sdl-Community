using System;

namespace Sdl.Community.Parser
{
    [Serializable]
    public class TranslationOrigin
    {
        public bool IsRepeated { get; set; }
        public bool IsStructureContextMatch { get; set; }

        public int MatchPercentage { get; set; }

        public string OriginSystem { get; set; }
        public string OriginType { get; set; }

        public string RepetitionTableId { get; set; }
        public string TextContextMatchLevel { get; set; }

        public TranslationOrigin()
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
