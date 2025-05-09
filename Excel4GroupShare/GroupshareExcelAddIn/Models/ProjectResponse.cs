using System.Collections.Generic;

namespace GroupshareExcelAddIn.Models
{
    public class ProjectResponse
    {
        public int Count { get; set; }
        public List<ProjectDetailsResponse> Items { get; set; }
    }
}