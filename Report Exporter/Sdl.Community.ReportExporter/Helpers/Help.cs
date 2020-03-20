using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Sdl.Community.ReportExporter.Model;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.ReportExporter.Helpers
{
	public static class Help
	{
		public static readonly Log Log = Log.Instance;

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
			if (installedStudioVersion.Equals(15))
			{
				projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					@"Studio 2019\Projects\projects.xml");
			}
			return projectsPath;
		}

		public static Dictionary<string, LanguageDirection> LoadLanguageDirections(XmlDocument doc)
		{
			var languages = new Dictionary<string, LanguageDirection>();
			try
			{
				var languageeDirectionNode = doc.SelectNodes("/Project/LanguageDirections/LanguageDirection");

				if (languageeDirectionNode == null) return languages;
				foreach (var item in languageeDirectionNode)
				{
					var node = (XmlNode)item;
					if (node.Attributes == null) continue;
					var lang = new LanguageDirection
					{
						Guid = node.Attributes["Guid"].Value,
						TargetLang = CultureInfo.GetCultureInfo(node.Attributes["TargetLanguageCode"].Value)
					};
					languages.Add(lang.Guid, lang);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadLanguageDirections method: {ex.Message}\n {ex.StackTrace}");
			}
			return languages;
		}

		public static void LoadReports(XmlDocument doc, string projectPath, ProjectDetails project)
		{
			try
			{
				var projectInfo = GetProjectInfo(project.ProjectPath);

				if (project.LanguageAnalysisReportPaths != null)
				{
					project.LanguageAnalysisReportPaths.Clear();
				}

				var automaticTaskNode = doc.SelectNodes("/Project/Tasks/AutomaticTask");
				if (automaticTaskNode != null)
				{
					var reportsFolderExist = ReportsFolderExists(automaticTaskNode[0].BaseURI);
					if (reportsFolderExist)
					{
						foreach (var node in automaticTaskNode)
						{
							var task = (XmlNode)node;
							var reportNodes = task.SelectNodes("Reports/Report");

							if (reportNodes == null) continue;

							foreach (var reportNode in reportNodes)
							{
								var report = (XmlNode)reportNode;
								if (report.Attributes != null && report.Attributes["TaskTemplateId"].Value ==
									"Sdl.ProjectApi.AutomaticTasks.Analysis")
								{
									var reportLangDirectionId = report.Attributes["LanguageDirectionGuid"].Value;
									var languageDirectionsNode = doc.SelectNodes("Project/LanguageDirections/LanguageDirection");
									foreach (XmlNode langDirNode in languageDirectionsNode)
									{
										var fileLangGuid = langDirNode.Attributes["Guid"].Value;
										if (reportLangDirectionId.Equals(fileLangGuid))
										{
											var targetLangCode = langDirNode.Attributes["TargetLanguageCode"].Value;
											var langName = projectInfo?.TargetLanguages?.FirstOrDefault(n => n.IsoAbbreviation.Equals(targetLangCode));

											var reportPath = Path.Combine(projectPath, report.Attributes["PhysicalPath"].Value);
											if (project.LanguageAnalysisReportPaths == null)
											{
												project.LanguageAnalysisReportPaths = new Dictionary<string, string>();
											}
											project.LanguageAnalysisReportPaths.Add(langName?.DisplayName, reportPath);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadReports method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		public static bool ReportFileExist(string reportFolderPath)
		{
			try
			{
				if (Directory.Exists(reportFolderPath))
				{
					var files = Directory.GetFiles(reportFolderPath);
					var exist = files.Any(file => file.Contains("Analyze Files"));
					if (exist)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ReportFileExist method: {ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}

		private static ProjectInfo GetProjectInfo(string projectPath)
		{
			var fileBasedProject = new FileBasedProject(projectPath);
			var projectInfo = fileBasedProject.GetProjectInfo();

			return projectInfo;
		}

		private static bool ReportsFolderExists(string projectFolderPath)
		{
			var projectPath = new Uri(projectFolderPath).LocalPath;
			var reportFolderPath =Path.Combine(projectPath.Substring(0, projectPath.LastIndexOf(@"\", StringComparison.Ordinal)), "Reports");

			return ReportFileExist(reportFolderPath);
		}

	}
}
