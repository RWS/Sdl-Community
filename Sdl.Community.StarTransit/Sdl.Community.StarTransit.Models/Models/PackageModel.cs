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
        public List<ProjectTemplateInfo> ProjectTemplate { get; set; }
        public CultureInfo SourceLanguage { get; set; }
        public CultureInfo TargetLanguage { get; set; }
        public string Location { get; set; }
        public string[] Files { get; set; }
    }
}
