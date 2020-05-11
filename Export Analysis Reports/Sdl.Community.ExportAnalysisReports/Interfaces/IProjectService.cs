using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IProjectService
	{
		string ProjectsXmlPath { get; set; }

		ProjectsController ProjectController { get; set; }

		List<string> AddFilePaths(List<string> studioProjectsPaths);

		ProjectDetails CreateProjectDetails(XmlNode projNode, bool isSingleFileProject, string reportsFolderPath);

		ProjectInfo GetProjectInfo(string projectPath);

		void RemoveSingleFileProjectLanguages(Dictionary<string, bool> languagesDictionary, BindingList<LanguageDetails> languages);

		void SetProjectLanguages(List<ProjectDetails> projectDetails, bool isChecked, string languageName);

		ProjectDetails GetExternalProjectDetails(string path, string reportFolderPath);
	}
}