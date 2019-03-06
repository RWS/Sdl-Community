using System.Collections.Generic;
using Sdl.Core.Globalization;

namespace Sdl.Community.SignoffVerifySettings.Model
{
	public class ProjectInfoReportModel
	{
		public string ProjectName { get; set; }

		// used to display the RunAt value from the "Verify Files .xml" report generated for all project files
		public string RunAt{ get; set; }

		public Language SourceLanguage { get; set; }

		public List<Language> TargetLanguages { get; set; }
		
		// used to display target files information
		public List<LanguageFileXmlNodeModel> LanguageFileXmlNodeModels { get; set; }
		
		// used to display phase information for each target file
		public List<PhaseXmlNodeModel> PhaseXmlNodeModels { get; set; }
	}
}