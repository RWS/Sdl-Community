using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.Projects.Activities
{

    [Serializable]
    public class Activity: ICloneable
    {
        public enum Status
        {
            New,
            Confirmed
        }
        
        public int Id { get; set; }      
        public string Name { get; set; }      
        public string Description { get; set; }       
        public Status ActivityStatus { get; set; } // New; Confirmed
       
        public bool Billable { get; set; }

        public List<DocumentActivities> Activities { get; set; }

        public int ProjectId { get; set; }
        public int CompanyProfileId { get; set; }    

        public DateTime? Started { get; set; }
        public DateTime? Stopped { get; set; }
         
        public bool LanguageRateChecked { get; set; }                              
        public bool HourlyRateChecked { get; set; }
        public bool CustomRateChecked { get; set; }

        public bool IsChecked { get; set; }

        public ActivityRates DocumentActivityRates { get; set; }
        public ComparisonSettings ComparisonOptions { get; set; }
        public QualityMetricReportSettings MetricReportSettings { get; set; }


      
        public Activity()
        {

            Id = -1;
            Name = string.Empty;
            Description = string.Empty;
            ActivityStatus = Status.New;
            Billable = true;

            Activities = new List<DocumentActivities>();

            ProjectId = -1;
            CompanyProfileId = -1;

            Started = null;
            Stopped = null;

            LanguageRateChecked = false;
            HourlyRateChecked = false;
            CustomRateChecked = false;

            IsChecked = false;

            DocumentActivityRates = new ActivityRates();
            ComparisonOptions = new ComparisonSettings();
            MetricReportSettings = new QualityMetricReportSettings();

        
           
        }

        public object Clone()
        {
            var activity = new Activity
            {
                Id = Id,
                Name = Name,
                Description = Description,
                ActivityStatus = ActivityStatus,
                Billable = Billable,
                Activities = new List<DocumentActivities>()
            };


            foreach (var td in Activities)
                activity.Activities.Add((DocumentActivities)td.Clone());

         
            activity.ProjectId = ProjectId;
            activity.CompanyProfileId = CompanyProfileId;
           
            activity.Started = Started;
            activity.Stopped = Stopped;

            activity.LanguageRateChecked = LanguageRateChecked;
            activity.HourlyRateChecked = HourlyRateChecked;
            activity.CustomRateChecked = CustomRateChecked;

            activity.IsChecked = IsChecked;

            activity.DocumentActivityRates = (ActivityRates)DocumentActivityRates.Clone();
            activity.ComparisonOptions = (ComparisonSettings)ComparisonOptions.Clone();
            activity.MetricReportSettings = (QualityMetricReportSettings)MetricReportSettings.Clone();

            

            return activity;

        }
    }
}
