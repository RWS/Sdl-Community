using System;

namespace Sdl.Community.Structures.Profile
{

    [Serializable]
    public class CompanyProfileRate : ICloneable
    {
        public int Id { get; set; }        
        public int CompanyProfileId { get; set; }
        public bool LanguageRateAutoAdd { get; set; }
        public int LanguageRateId { get; set; }
        public bool HourlyRateAutoAdd { get; set; }
        public decimal HourlyRateRate { get; set; }
        public string HourlyRateCurrency { get; set; }

        public CompanyProfileRate()
        {
            Id = -1;
            CompanyProfileId = -1;
            LanguageRateAutoAdd = false;
            LanguageRateId = -1;
            HourlyRateAutoAdd = false;
            HourlyRateCurrency = string.Empty;
            HourlyRateRate = 0;
        }
        public object Clone()
        {
            return (CompanyProfileRate)MemberwiseClone();
        }


    }
}
