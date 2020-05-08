using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IStudioService
	{
		ProjectInfo GetProjectInfo(string projectPath);
		BindingList<ProjectDetails> SetProjectDetails(List<ProjectDetails> projects, BindingList<ProjectDetails> newProjectDetails);
		void RemoveSingleFileProjectLanguages(Dictionary<string, bool> languagesDictionary, BindingList<LanguageDetails> languages);
		List<string> AddFilePaths(List<string> studioProjectsPaths);
		ProjectDetails CreateProjectDetails(XmlNode projNode, bool isSingleFileProject, string reportsFolderPath);

		string ProjectsXmlPath { get; set; }
	}
}
