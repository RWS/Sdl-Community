using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Shared.Models
{
    public class PackageModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectTemplateInfo ProjectTemplate { get; set; }
        public List<ProjectTemplateInfo> StudioTemplates { get; set; }
        public CultureInfo SourceLanguage { get; set; }
        public List<CultureInfo> TargetLanguage { get; set; }
        public string Location { get; set; }
        public DateTime? DueDate { get; set; }
        public bool HasDueDate { get; set; }
        public string[] SourceFiles { get; set; }
        public string[] TargetFiles { get; set; }
    }
}
