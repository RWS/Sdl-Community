using System;

namespace Sdl.Community.Structures.Projects.Activities
{
    public class ActivityReports : ICloneable
    {
        public int Id { get; set; }
        public int ProjectActivityId { get; set; }
        public string ReportOverview { get; set; }
        public string ReportMetrics { get; set; }

        public ActivityReports()
        {
            Id = -1;
            ProjectActivityId = -1;
            ReportOverview = string.Empty;
            ReportMetrics = string.Empty;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
