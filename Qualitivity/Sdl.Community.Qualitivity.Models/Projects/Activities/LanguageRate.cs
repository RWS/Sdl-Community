using System;
using System.Xml.Serialization;
using Sdl.Community.Structures.Rates.Base;

namespace Sdl.Community.Structures.Projects.Activities
{
    [Serializable]
    public class LanguageRate : IRate, ICloneable
    {

        public string Id { get; set; }
        [XmlIgnore]
        public int ProjectActivityRateId { get; set; }
        [XmlIgnore]
        public int ProjectActivityId { get; set; }

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
            Id = Guid.NewGuid().ToString();
            ProjectActivityRateId = -1;
            ProjectActivityId = -1;

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
