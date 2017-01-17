using System;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Settings
{
    [Serializable]
    public class PenaltySettings
    {
        public int MissingFormattingPenalty { get; set; }
        public int DifferentFormattingPenalty { get; set; }
        

        public PenaltySettings()
        {
            MissingFormattingPenalty = 1;
            DifferentFormattingPenalty = 1;
        }
    }
}
