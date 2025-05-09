using System;

namespace GroupshareExcelAddIn.Models
{
    public class ProjectDetailsResponse
    {
        public DateTime? CompletedAt { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CustomerName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Name { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationPath { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectId { get; set; }
        public string SourceLanguage { get; set; }
        public int Status { get; set; }
        public string TargetLanguage { get; set; }
    }
}