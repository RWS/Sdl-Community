using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;

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

		// used to display the Materials information
		public List<TranslationProviderCascadeEntry> TranslationMemories { get; set; }
		public List<Termbase> Termbases { get; set; }
		public string RegexRules { get; set; }
		public string CheckRegexRules { get; set; }
		public string QACheckerRanResult { get; set; }

		// used to display the settings applied for the QA Verification
		public List<QAVerificationSettingsModel> QAVerificationSettingsModels { get; set; }

		// used to display when and on which file the NumberVerifier had been executed
		public NumberVerifierSettingsModel NumberVerifierSettingsModel { get; set; }
	}
}