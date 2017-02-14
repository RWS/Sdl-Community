using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.Projects.Activities
{
    [Serializable]
    public class ActivityRates : ICloneable
    {
        
        public int Id { get; set; }
        public int ProjectActivityId { get; set; }
        
        public int LanguageRateId { get; set; }        
        public string LanguageRateName { get; set; }
        public string LanguageRateDescription { get; set; }
        public string LanguageRateCurrency { get; set; }
        public double LanguageRateTotal { get; set; }
        public List<LanguageRate> LanguageRates { get; set; }

        public string HourlyRateName { get; set; }
        public string HourlyRateDescription { get; set; }
        public double HourlyRateRate { get; set; }
        public double HourlyRateQuantity { get; set; }
        public string HourlyRateCurrency { get; set; }
        public double HourlyRateTotal { get; set; }

        public string CustomRateName { get; set; }
        public string CustomRateDescription { get; set; }
        public string CustomRateCurrency { get; set; }
        public double CustomRateTotal { get; set; }


        public ActivityRates()
        {
            Id = -1;
            ProjectActivityId = -1;

            LanguageRateId = -1;
            LanguageRateName = string.Empty;
            LanguageRateDescription = "[Language Rate]";
            LanguageRateCurrency = string.Empty;
            LanguageRateTotal = 0;
            LanguageRates = new List<LanguageRate>();

            HourlyRateName = string.Empty;
            HourlyRateDescription = "[Hourly Rate]";
            HourlyRateRate = 0;
            HourlyRateQuantity = 0;
            HourlyRateCurrency = string.Empty;
            HourlyRateTotal = 0;

            CustomRateName = string.Empty;
            CustomRateDescription = "[Custom Rate]";
            CustomRateCurrency = string.Empty;
            CustomRateTotal = 0;
    
        }
        public object Clone()
        {
            var activityRate = new ActivityRates();

            activityRate.Id = Id;
            activityRate.ProjectActivityId = ProjectActivityId;

            activityRate.LanguageRateId = LanguageRateId;
            activityRate.LanguageRateName = LanguageRateName;
            activityRate.LanguageRateDescription = LanguageRateDescription;
            activityRate.LanguageRateCurrency = LanguageRateCurrency;
            activityRate.LanguageRateTotal = LanguageRateTotal;

            activityRate.LanguageRates = new List<LanguageRate>();
            foreach (var languageRate in LanguageRates)
                activityRate.LanguageRates.Add((LanguageRate)languageRate.Clone());


            activityRate.HourlyRateName = HourlyRateName;
            activityRate.HourlyRateDescription = HourlyRateDescription;
            activityRate.HourlyRateRate = HourlyRateRate;
            activityRate.HourlyRateQuantity = HourlyRateQuantity;
            activityRate.HourlyRateCurrency = HourlyRateCurrency;
            activityRate.HourlyRateTotal = HourlyRateTotal;

            activityRate.CustomRateName = CustomRateName;
            activityRate.CustomRateDescription = CustomRateDescription;
            activityRate.CustomRateCurrency = CustomRateCurrency;
            activityRate.CustomRateTotal = CustomRateTotal;

            return activityRate;
        }


    }
}
