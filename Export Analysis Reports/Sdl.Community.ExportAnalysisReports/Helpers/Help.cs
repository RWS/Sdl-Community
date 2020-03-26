using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.ExportAnalysisReports.Helpers
{
	public static class Help
	{
		public static readonly Log Log = Log.Instance;
		public static readonly string CommunityFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "ExportAnalysisReports");
		public static readonly string JsonPath = Path.Combine(CommunityFolderPath, "ExportAnalysisReportSettings.json");

		public static string GetStudioProjectsPath()
		{
			try
			{
				var shortStudioVersion = GetInstalledStudioShortVersion();
				if (string.IsNullOrEmpty(shortStudioVersion))
				{
					return string.Empty;
				}

				var projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $@"Studio {shortStudioVersion}\Projects\projects.xml");
				return projectsPath;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"GetStudioProjectsPath method: {ex.Message}\n {ex.StackTrace}");
			}
			return string.Empty;
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
					if (files.Any(file => file.Contains("Analyze Files")))
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

		public static void SaveExportPath(string reportOutputPath)
		{
			if (!string.IsNullOrEmpty(reportOutputPath))
			{
				if (!Directory.Exists(CommunityFolderPath))
				{
					Directory.CreateDirectory(CommunityFolderPath);
				}

				var jsonExportPath = new JsonSettings { ExportPath = reportOutputPath };
				var jsonResult = JsonConvert.SerializeObject(jsonExportPath);

				if (File.Exists(JsonPath))
				{
					File.Delete(JsonPath);
				};
				File.Create(JsonPath).Dispose();

				using (var tw = new StreamWriter(JsonPath, true))
				{
					tw.WriteLine(jsonResult);
					tw.Close();
				}
			}
		}

		public static string GetJsonReportPath(string jsonPath)
		{
			if (File.Exists(jsonPath))
			{
				using (var r = new StreamReader(jsonPath))
				{
					var json = r.ReadToEnd();
					var item = JsonConvert.DeserializeObject<JsonSettings>(json);
					if (!string.IsNullOrEmpty(item.ExportPath))
					{
						return item.ExportPath;
					}
				}
			}
			return string.Empty;
		}

		private static string GetInstalledStudioShortVersion()
		{
			var studioService = new StudioVersionService();
			return studioService?.GetStudioVersion()?.ShortVersion;
		}

		private static ProjectInfo GetProjectInfo(string projectPath)
		{
			var fileBasedProject = new FileBasedProject(projectPath);
			var projectInfo = fileBasedProject?.GetProjectInfo();

			return projectInfo;
		}

		private static bool ReportsFolderExists(string projectFolderPath)
		{
			var projectPath = new Uri(projectFolderPath).LocalPath;
			var reportFolderPath = Path.Combine(projectPath.Substring(0, projectPath.LastIndexOf(@"\", StringComparison.Ordinal)), "Reports");

			return ReportFileExist(reportFolderPath);
		}
	}
}