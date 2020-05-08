using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IStudioService
	{
		string ProjectsXmlPath { get; set; }

		List<string> AddFilePaths(List<string> studioProjectsPaths);

		ProjectDetails CreateProjectDetails(XmlNode projNode, bool isSingleFileProject, string reportsFolderPath);

		ProjectInfo GetProjectInfo(string projectPath);

		void RemoveSingleFileProjectLanguages(Dictionary<string, bool> languagesDictionary, BindingList<LanguageDetails> languages);

		BindingList<ProjectDetails> SetProjectDetails(List<ProjectDetails> projects, BindingList<ProjectDetails> newProjectDetails);
	}
}