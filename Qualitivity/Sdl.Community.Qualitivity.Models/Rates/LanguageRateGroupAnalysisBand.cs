using System;

namespace Sdl.Community.Structures.Rates
{

    [Serializable]
    public class LanguageRateGroupAnalysisBand : ICloneable
    {

        public int Id { get; set; }
        public int LanguageRateId { get; set; }
        public int PercentPm { get; set; }
        public int PercentCm { get; set; }
        public int PercentRep { get; set; }
        public int Percent100 { get; set; }
        public int Percent95 { get; set; }
        public int Percent85 { get; set; }
        public int Percent75 { get; set; }
        public int Percent50 { get; set; }
        public int PercentNew { get; set; }

        public LanguageRateGroupAnalysisBand()
        {
            Id = -1;
            LanguageRateId = -1;
            PercentPm = 0;
            PercentCm = 0;
            PercentRep = 0;
            Percent100 = 2;
            Percent95 = 20;
            Percent85 = 65;
            Percent75 = 75;
            Percent50 = 100;
            PercentNew = 100;
        }
        public object Clone()
        {
            return (LanguageRateGroupAnalysisBand)MemberwiseClone();
        }
    }
}
