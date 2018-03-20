using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ReportExporter.Model
{
	public class ProjectDetails
	{
		public ProjectDetails()
		{
		}

		public ProjectDetails(string projectName, string projectPath)
		{
			ProjectName = projectName;
			ProjectPath = projectPath;
		}

		public string Status { get; set; }
		public string ProjectName
		{
			get;
			set;
		}

		public string ProjectPath
		{
			get;
			set;
		}

		public string ProjectFolderPath => ProjectPath.Substring(0, ProjectPath.LastIndexOf("\\", StringComparison.Ordinal));

		public Dictionary<string, bool> LanguagesForPoject { get; set; }
		public bool ShouldBeExported { get; set; }
		public string ReportPath { get; set; }

		public string PathToAnalyseResult { get; set; }
		public override string ToString()
		{
			return ProjectName;
		}
	}
}
