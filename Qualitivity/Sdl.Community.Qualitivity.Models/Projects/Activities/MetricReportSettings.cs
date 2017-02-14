using System;

namespace Sdl.Community.Structures.Projects.Activities
{
   
    [Serializable]
    public class QualityMetricReportSettings : ICloneable
    {    
        public int Id { get; set; }
        public int ProjectActivityId { get; set; }
        public string MetricGroupName { get; set; }
        public int MaxSeverityValue { get; set; }
        public int MaxSeverityInValue { get; set; }
        public string MaxSeverityInType { get; set; }

        public QualityMetricReportSettings()
        {
            Id = -1;
            ProjectActivityId = -1;
            MetricGroupName = string.Empty;
            MaxSeverityValue = 10;
            MaxSeverityInValue = 1000;
            MaxSeverityInType = "words";
        }
      
        public object Clone()
        {
            return (QualityMetricReportSettings)MemberwiseClone();
        }
    }
}
