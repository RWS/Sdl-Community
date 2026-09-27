using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.ExportAnalysisReports.Model
{
	public class ProjectDetails
	{
		public string Status { get; set; }
		public string ProjectName { get; set; }
		public string ProjectPath { get; set; }
		public bool ShouldBeExported { get; set; }
		public string ReportPath { get; set; }
		public string ReportsFolderPath { get; set; }
		public bool IsSingleFileProject { get; set; }
		public Dictionary<string, bool> ProjectLanguages { get; set; }
		public Dictionary<string, string> LanguageAnalysisReportPaths { get; set; }

		public string ProjectFolderPath => Path.GetDirectoryName(ProjectPath);

		public override string ToString()
		{
			return ProjectName;
		}
	}
}
