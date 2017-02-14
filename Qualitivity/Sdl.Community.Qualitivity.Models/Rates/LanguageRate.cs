using System;
using Sdl.Community.Structures.Rates.Base;

namespace Sdl.Community.Structures.Rates
{

    [Serializable]
    public class LanguageRate : IRate, ICloneable
    {
        
        public int Id { get; set; }
        public int LanguageRateId { get; set; }

        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }

        public RoundType RndType { get; set; }


        public decimal BaseRate { get; set; }

        public decimal RatePm { get; set; }
        public decimal RateCm { get; set; }
        public decimal RateRep { get; set; }
        public decimal Rate100 { get; set; }
        public decimal Rate95 { get; set; }
        public decimal Rate85 { get; set; }
        public decimal Rate75 { get; set; }
        public decimal Rate50 { get; set; }
        public decimal RateNew { get; set; }

        public LanguageRate()
        {
            Id = -1;
            LanguageRateId = -1;
            SourceLanguage = string.Empty;
            TargetLanguage = string.Empty;
            RndType = RoundType.Round;

            BaseRate = 0;

            RatePm = 0;
            RateCm = 0;
            RateRep = 0;
            Rate100 = 0;
            Rate95 = 0;
            Rate85 = 0;
            Rate75 = 0;
            Rate50 = 0;
            RateNew = 0;
        }
        public object Clone()
        {
            return (LanguageRate)MemberwiseClone();
        }


    }
}
