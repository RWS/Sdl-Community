using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sdl.Community.ReportExporter.Model;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using TaskStatus = Sdl.ProjectAutomation.Core.TaskStatus;

namespace Sdl.Community.ReportExporter.Helpers
{
	public static class Help
	{
		public static int GetInstalledStudioMajorVersion()
		{
			var studioService = new StudioVersionService();
			return studioService.GetStudioVersion().ExecutableVersion.Major;
		}

		public static string GetStudioProjectsPath()
		{
			var installedStudioVersion = GetInstalledStudioMajorVersion();
			var projectsPath = string.Empty;

			if (installedStudioVersion.Equals(14))
			{
				projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					@"Studio 2017\Projects\projects.xml");
			}
			if (installedStudioVersion.Equals(12))
			{
				projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					@"Studio 2015\Projects\projects.xml");
			}

			return projectsPath;
		}

		public static Dictionary<string, LanguageDirection> LoadLanguageDirections(XmlDocument doc)
		{
			var languages = new Dictionary<string, LanguageDirection>();
			var languageeDirectionNode = doc.SelectNodes("/Project/LanguageDirections/LanguageDirection");

			if (languageeDirectionNode == null) return languages;
			foreach (var item in languageeDirectionNode)
			{
				var node = (XmlNode) item;
				if (node.Attributes == null) continue;
				var lang = new LanguageDirection
				{
					Guid = node.Attributes["Guid"].Value,
					TargetLang = CultureInfo.GetCultureInfo(node.Attributes["TargetLanguageCode"].Value)
				};
				languages.Add(lang.Guid, lang);
			}

			return languages;
		}

		public static void LoadReports(XmlDocument doc, string projectPath, Dictionary<string, LanguageDirection> languages)
		{
			var automaticTaskNode = doc.SelectNodes("/Project/Tasks/AutomaticTask");
			if (automaticTaskNode != null)
			{
				ReportsFolderExists(automaticTaskNode[0].BaseURI);
				foreach (var node in automaticTaskNode)
				{
					var task = (XmlNode) node;
					var reportNodes = task.SelectNodes("Reports/Report");

					if (reportNodes == null) continue;

					foreach (var reportNode in reportNodes)
					{
						var report = (XmlNode) reportNode;
						if (report.Attributes != null && report.Attributes["TaskTemplateId"].Value ==
						    "Sdl.ProjectApi.AutomaticTasks.Analysis")
						{
							//always overwrite the previous path, we are only interested into last analyze
							languages[report.Attributes["LanguageDirectionGuid"].Value].PathToReport =
								projectPath + "\\" + report.Attributes["PhysicalPath"].Value;
						}
					}
				}
			}
		}

		private static void ReportsFolderExists(string projectFolderPath)
		{
			var projectPath = new Uri(projectFolderPath).LocalPath;//Path.GetFullPath(projectFolderPath.Replace(@"file:///", string.Empty));
			var reportFolderPath =Path.Combine(projectPath.Substring(0, projectPath.LastIndexOf(@"\", StringComparison.Ordinal)), "Reports");
			if (!Directory.Exists(reportFolderPath))
			{
				RunAnalyseBatchTask(Path.GetFullPath(projectPath));

			}
			else
			{
				var files = Directory.GetFiles(reportFolderPath);
				var exist = files.Any(file => file.Contains("Analyze Files"));
				if (!exist)
				{
					RunAnalyseBatchTask(Path.GetFullPath(projectPath));
				}
			}
		}

		private static void RunAnalyseBatchTask(string projectFolderPath)
		{
			var studioProjects = SdlTradosStudio.Application.GetController<ProjectsController>().GetAllProjects();
			
			var project = studioProjects.FirstOrDefault(p => p.FilePath.Equals(projectFolderPath));

			if (project != null)
			{
				var projectFilesList = project.GetTargetLanguageFiles().GetIds();

				var analyseTask = project.RunAutomaticTask(projectFilesList, AutomaticTaskTemplateIds.AnalyzeFiles, (sender, e)
						=>
					{
					}
					, (sender, e) =>
					{

					});

				//	if (analyseTask.Status == TaskStatus.Completed)
				//	{
				//		var reportNodes = node.SelectNodes(@"Reports/Report");

				//		return reportNodes;
				//	}
				//}
				//return null;
			}
		}

	}
}
