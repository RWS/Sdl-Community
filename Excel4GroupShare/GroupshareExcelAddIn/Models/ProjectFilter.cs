using Newtonsoft.Json;
using System;

namespace GroupshareExcelAddIn.Models
{
    public class ProjectFilter
    {
        public ProjectFilter()
        {
        }

        public ProjectFilter(string orgPath, DateRange dateRange, ProjectStatus projectStatus, bool includeSubOrgs, bool includePhasesAndAssignees)
        {
            IncludeSubOrgs = includeSubOrgs;
            IncludePhasesAndAssignees = includePhasesAndAssignees;
            OrgPath = orgPath;
            StartDeliveryDate = dateRange.StartDeliveryDate;
            EndDeliveryDate = dateRange.EndDeliveryDate;
            StartPublishingDate = dateRange.StartPublishingDate;
            EndPublishingDate = dateRange.EndPublishingDate;

            Status = (int)projectStatus;
        }

        public static ProjectFilter NoRestrictionFilter { get; } = new ProjectFilter
        {
            EndDeliveryDate = null,
            EndPublishingDate = null,
            IncludePhasesAndAssignees = false,
            IncludeSubOrgs = true,
            OrgPath = "\u002F",
            ProjectName = null,
            StartDeliveryDate = null,
            StartPublishingDate = null,
            Status = 31
        };

        [JsonProperty(PropertyName = "dueEnd")]
        public DateTime? EndDeliveryDate { get; set; }

        [JsonProperty(PropertyName = "createdEnd")]
        public DateTime? EndPublishingDate { get; set; }

        [JsonIgnore]
        public bool IncludePhasesAndAssignees { get; set; }

        [JsonProperty(PropertyName = "includeSubOrgs")]
        public bool IncludeSubOrgs { get; set; }

        [JsonIgnore]
        public bool IsUnrestrictive => Equals(NoRestrictionFilter);

        [JsonProperty(PropertyName = "orgPath")]
        public string OrgPath { get; set; }

        [JsonProperty(PropertyName = "projectName")]
        public string ProjectName { get; set; }

        [JsonProperty(PropertyName = "dueStart")]
        public DateTime? StartDeliveryDate { get; set; }

        [JsonProperty(PropertyName = "createdStart")]
        public DateTime? StartPublishingDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        private bool Equals(ProjectFilter other)
        {
            return Nullable.Equals(EndDeliveryDate, other.EndDeliveryDate) && Nullable.Equals(EndPublishingDate, other.EndPublishingDate) && IncludeSubOrgs == other.IncludeSubOrgs && OrgPath == other.OrgPath && ProjectName == other.ProjectName && Nullable.Equals(StartDeliveryDate, other.StartDeliveryDate) && Nullable.Equals(StartPublishingDate, other.StartPublishingDate) && Status == other.Status && IncludePhasesAndAssignees == other.IncludePhasesAndAssignees;
        }
    }
}