using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IStudioService
	{
		string GetStudioProjectsPath();
		ProjectInfo GetProjectInfo(string projectPath);
		BindingList<ProjectDetails> SetProjects(List<ProjectDetails> projects, BindingList<ProjectDetails> newProjectDetails);
		void SetProjectLanguages(ProjectDetails project, Dictionary<string, LanguageDirection> languages);
		void RemoveLanguages(Dictionary<string, bool> languagesDictionary, BindingList<LanguageDetails> languages);
		Dictionary<string, LanguageDirection> LoadLanguageDirections(XmlDocument doc);
		void FillLanguages(BindingList<LanguageDetails> languages, BindingList<ProjectDetails> projects, ProjectDetails selectedProject);
	}
}
