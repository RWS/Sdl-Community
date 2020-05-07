using System.Collections.Generic;
using System.ComponentModel;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IStudioService
	{
		string GetStudioProjectsPath();
		ProjectInfo GetProjectInfo(string projectPath);
		void SetLanguagesForProject(ProjectDetails project, Dictionary<string, LanguageDirection> languages);
		void RemoveLanguages(Dictionary<string, bool> languagesDictionary, BindingList<LanguageDetails> languages);
		BindingList<ProjectDetails> SetProjects(List<ProjectDetails> projects, BindingList<ProjectDetails> newProjectDetails);
	}
}
