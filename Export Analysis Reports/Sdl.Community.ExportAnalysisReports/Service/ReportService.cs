using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Sdl.Community.ExportAnalysisReports.Helpers;
using Sdl.Community.ExportAnalysisReports.Interfaces;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class ReportService : IReportService
	{
		private readonly IMessageBoxService _messageBoxService;
		private readonly IStudioService _studioService;
		private readonly string _communityFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "ExportAnalysisReports");

		public static readonly Log Log = Log.Instance;
		public string JsonPath { get; set; }

		public ReportService(IMessageBoxService messageBoxService, IStudioService studioService)
		{
			_messageBoxService = messageBoxService;
			_studioService = studioService;
			JsonPath = Path.Combine(_communityFolderPath, "ExportAnalysisReportSettings.json");
		}

		public void LoadReports(XmlDocument doc, ProjectDetails project)
		{
			try
			{
				var projectInfo = _studioService.GetProjectInfo(project.ProjectPath);

				project.LanguageAnalysisReportPaths?.Clear();

				var automaticTaskNode = doc.SelectNodes("/Project/Tasks/AutomaticTask");
				if (automaticTaskNode != null)
				{
					var reportsFolderExist = ReportsFolderExists(automaticTaskNode[0].BaseURI, project.ReportsFolderPath);
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
								if (report.Attributes != null && report.Attributes["TaskTemplateId"].Value.Equals("Sdl.ProjectApi.AutomaticTasks.Analysis"))
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

											var reportPath = Path.Combine(project.ProjectFolderPath, report.Attributes["PhysicalPath"].Value);
											if (!string.IsNullOrEmpty(project.ReportsFolderPath))
											{
												reportPath = Path.Combine(project.ReportsFolderPath, Path.GetFileName(report.Attributes["PhysicalPath"].Value));
											}
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

		public string GetJsonReportPath(string jsonPath)
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

		public Dictionary<string, LanguageDirection> LoadLanguageDirections(XmlDocument doc)
		{
			var languages = new Dictionary<string, LanguageDirection>();
			try
			{
				var languagesDirectionNode = doc.SelectNodes("/Project/LanguageDirections/LanguageDirection");

				if (languagesDirectionNode == null) return languages;
				foreach (var item in languagesDirectionNode)
				{
					var node = (XmlNode)item;
					if (node.Attributes == null) continue;
					var lang = new LanguageDirection
					{
						Guid = node.Attributes["Guid"].Value,
						TargetLang = CultureInfo.GetCultureInfo(node.Attributes["TargetLanguageCode"].Value)
					};
					if (!languages.ContainsKey(lang.Guid))
					{
						languages.Add(lang.Guid, lang);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadLanguageDirections method: {ex.Message}\n {ex.StackTrace}");
			}
			return languages;
		}

		public bool IsSameReportPath(string reportOutputPath)
		{
			var jsonReportPath = GetJsonReportPath(JsonPath);
			if (!string.IsNullOrEmpty(jsonReportPath) && !string.IsNullOrEmpty(reportOutputPath) && jsonReportPath.Equals(reportOutputPath))
			{
				return true;
			}

			return false;
		}

		public bool ReportFileExist(string reportFolderPath)
		{
			try
			{
				var fileName = Path.GetFileName(Path.GetDirectoryName(reportFolderPath));
				if (Directory.Exists(reportFolderPath))
				{
					var files = Directory.GetFiles(reportFolderPath);
					if (files.Any(file => file.Contains("Analyze Files")))
					{
						return true;
					}
					_messageBoxService.ShowInformationMessage(string.Format(PluginResources.ExecuteAnalyzeBatchTask_Message, fileName), PluginResources.InformativeLabel);
					return false;
				}
				if (!string.IsNullOrEmpty(fileName) && fileName.Contains("ProjectFiles"))
				{
					fileName = Path.GetFileNameWithoutExtension(fileName);
					return !string.IsNullOrEmpty(fileName);
				}
				_messageBoxService.ShowInformationMessage(string.Format(PluginResources.ExecuteAnalyzeBatchTask_Message, fileName), PluginResources.InformativeLabel);
				return false;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ReportFileExist method: {ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}

		public void SaveExportPath(string reportOutputPath)
		{
			if (!string.IsNullOrEmpty(reportOutputPath))
			{
				Directory.CreateDirectory(_communityFolderPath);

				var jsonExportPath = new JsonSettings { ExportPath = reportOutputPath };
				var jsonResult = JsonConvert.SerializeObject(jsonExportPath);

				if (File.Exists(JsonPath))
				{
					File.Delete(JsonPath);
				}

				File.Create(JsonPath).Dispose();

				using (var tw = new StreamWriter(JsonPath, true))
				{
					tw.WriteLine(jsonResult);
					tw.Close();
				}
			}
		}

		private bool ReportsFolderExists(string projectFolderPath, string reportsFolderPath)
		{
			if (!string.IsNullOrEmpty(reportsFolderPath))
			{
				return ReportFileExist(reportsFolderPath);
			}
			var projectPath = new Uri(projectFolderPath).LocalPath;
			reportsFolderPath = Path.Combine(projectPath.Substring(0, projectPath.LastIndexOf(@"\", StringComparison.Ordinal)), "Reports");
			return ReportFileExist(reportsFolderPath);
		}
	}
}
