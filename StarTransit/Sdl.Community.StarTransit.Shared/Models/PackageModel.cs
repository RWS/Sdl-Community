using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Shared.Models
{
	public class PackageModel
	{
		//Info from transit prj file 
		//Added in the new ui
        public string Name { get; set; }
        public CultureInfo SourceLanguage { get; set; }
        public Image SourceFlag { get; set; }
        public bool PackageContainsTms { get; set; }
        public Language[] TargetLanguages { get; set; }

		public string Description { get; set; } // TODO: Remove this for final implementation. This is no used anywhere we don't have description in the packages
        public ProjectTemplateInfo ProjectTemplate { get; set; }
        public List<ProjectTemplateInfo> StudioTemplates { get; set; }
        public string Location { get; set; }
        public DateTime? DueDate { get; set; }
        public bool HasDueDate { get; set; }
        public Customer Customer { get; set; }
        public List<LanguagePair> LanguagePairs { get; set; }
        public string PathToPrjFile { get; set; }
		public Dictionary<string,int> TMPenalties { get; set; }
		public List<string> MTMemories { get; set; }
	}
}