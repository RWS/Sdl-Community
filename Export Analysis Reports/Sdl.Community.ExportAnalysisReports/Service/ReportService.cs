using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Sdl.Community.ExportAnalysisReports.Helpers;
using Sdl.Community.ExportAnalysisReports.Interfaces;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class ReportService : IReportService
	{
		public static readonly Log Log = Log.Instance;
		private readonly string _communityFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "ExportAnalysisReports");
		private readonly IMessageBoxService _messageBoxService;
		private readonly IStudioService _studioService;

		public ReportService(IMessageBoxService messageBoxService, IStudioService studioService)
		{
			_messageBoxService = messageBoxService;
			_studioService = studioService;
		}

		public string JsonPath => Path.Combine(_communityFolderPath, "ExportAnalysisReportSettings.json");
		public string ReportsFolderPath { get; set; }

		/// <summary>
		/// Check if the report was successfully generated
		/// </summary>
		/// <param name="reportOutputPath"></param>
		/// <param name="isChecked"></param>
		/// <param name="optionalInformation"></param>
		/// <param name="projects"></param>
		/// <returns></returns>
		public bool GenerateReportFile(BindingList<ProjectDetails> projects, OptionalInformation optionalInformation, string reportOutputPath, bool isChecked)
		{
			try
			{
				Directory.CreateDirectory(reportOutputPath);
				var projectsToBeExported = projects.Where(p => p.ShouldBeExported).ToList();
				var areCheckedLanguages = projectsToBeExported.Any(p => p.PojectLanguages.Any(l => l.Value));
				if (areCheckedLanguages || projectsToBeExported.Count < 1)
				{
					foreach (var project in projectsToBeExported)
					{
						// check which languages to export
						if (project.PojectLanguages == null) continue;
						var checkedLanguages = project.PojectLanguages.Where(l => l.Value).ToList();

						foreach (var languageReport in checkedLanguages)
						{
							if (string.IsNullOrEmpty(project.ReportPath))
							{
								project.ReportPath = reportOutputPath;
							}

							WriteReportFile(project, optionalInformation, languageReport, isChecked);
						}
					}
					return true;
				}
				_messageBoxService.ShowInformationMessage(PluginResources.SelectLanguage_Export_Message, PluginResources.ExportResult_Label);
				return false;
			}
			catch (Exception exception)
			{
				Log.Logger.Error($"GenerateReport method: {exception.Message}\n {exception.StackTrace}");
				throw;
			}
		}

		/// <summary>
		/// Get report output path from Json file
		/// </summary>
		/// <param name="jsonPath"></param>
		/// <returns></returns>
		public string GetJsonReportPath(string jsonPath)
		{
			try
			{
				if (!File.Exists(jsonPath)) return string.Empty;
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
			catch (Exception ex)
			{
				Log.Logger.Error($"GetJsonReportPath method: {ex.Message}\n {ex.StackTrace}");
			}

			return string.Empty;
		}

		/// <summary>
		/// Check if the exported path is the same
		/// </summary>
		/// <param name="reportOutputPath"></param>
		/// <returns></returns>
		public bool IsSameReportPath(string reportOutputPath)
		{
			var jsonReportPath = GetJsonReportPath(JsonPath);
			return !string.IsNullOrEmpty(jsonReportPath) && !string.IsNullOrEmpty(reportOutputPath) && jsonReportPath.Equals(reportOutputPath);
		}

		/// <summary>
		/// Check if the report folder exists
		/// </summary>
		/// <param name="projectInfoNode"></param>
		/// <param name="projectXmlPath"></param>
		/// <returns></returns>
		public bool ReportFolderExist(XmlNode projectInfoNode, string projectXmlPath)
		{
			try
			{
				var filePath = SetProjectFilePath(projectInfoNode, projectXmlPath);
				return ReportFileExist(filePath);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ReportFolderExist method: {ex.Message}\n {ex.StackTrace}");
			}

			return false;
		}

		/// <summary>
		///  Save report output path within json file
		/// (It is saved within a json file, because it is a general path and is not related to an individual project)
		/// </summary>
		/// <param name="reportOutputPath"></param>
		public void SaveExportPath(string reportOutputPath)
		{
			if (string.IsNullOrEmpty(reportOutputPath)) return;
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

		/// <summary>
		/// Set report information using the project xml document
		/// </summary>
		/// <param name="project"></param>
		public void SetReportInformation(ProjectDetails project)
		{
			try
			{
				var doc = new XmlDocument();
				doc.Load(project.ProjectPath);

				var projectInfo = _studioService.GetProjectInfo(project.ProjectPath);
				project.LanguageAnalysisReportPaths?.Clear();

				var automaticTaskNode = doc.SelectNodes("/Project/Tasks/AutomaticTask");
				if (automaticTaskNode == null) return;
				var reportsFolderExist = ReportsFolderExists(project.ReportsFolderPath);
				if (reportsFolderExist)
				{
					foreach (var node in automaticTaskNode)
					{
						var task = (XmlNode)node;
						var reportNodes = task.SelectNodes("Reports/Report");

						if (reportNodes == null) continue;

						foreach (var reportNode in reportNodes)
						{
							ConfigureReportDetails(project, projectInfo, (XmlNode) reportNode, doc);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadReports method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		public List<ProjectDetails> GetExternalProjectReportInfo(string fileName)
		{
			var externalProjInfoList = new List<ProjectDetails>();
			try
			{
				var projectsPathList = Directory.GetFiles(fileName, "*.sdlproj", SearchOption.AllDirectories);
				foreach (var projectPath in projectsPathList)
				{
					var projDirectoryName = Path.GetDirectoryName(projectPath);
					if (string.IsNullOrEmpty(projDirectoryName)) continue;
					var reportFolderPath = Path.Combine(projDirectoryName, "Reports");
					if (!Directory.Exists(reportFolderPath))
					{
						// check for the single file project report's path
						var projectName = Path.GetFileNameWithoutExtension(projectPath);
						reportFolderPath = Path.Combine(projDirectoryName, $"{projectName}.ProjectFiles", "Reports");
					}

					if (!ReportFileExist(reportFolderPath)) continue;
					var projectDetails = ProjectInformation.GetExternalProjectDetails(projectPath, reportFolderPath);

					SetReportInformation(projectDetails);
					externalProjInfoList.Add(projectDetails);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetExternalProjectReportInfo method: {ex.Message}\n {ex.StackTrace}");
			}

			return externalProjInfoList;
		}

		// Set the paths for the projec files
		private string SetProjectFilePath(XmlNode projectInfoNode, string projectXmlPath)
		{
			var filePath = string.Empty;
			try
			{
				if (projectInfoNode?.Attributes?["ProjectFilePath"] != null)
				{
					filePath = projectInfoNode.Attributes["ProjectFilePath"]?.Value;
					if (!Path.IsPathRooted(filePath))
					{
						//project is located inside "Projects" folder in Studio
						var projectsFolderPath = Path.GetDirectoryName(projectXmlPath);

						// .Substring is needed here to keep the selection of projectNamePath correctly, ex: Samples\Sample Project
						var projectNamePath = filePath.Substring(0, filePath.LastIndexOf(@"\", StringComparison.Ordinal));
						if (!string.IsNullOrEmpty(projectsFolderPath) && !string.IsNullOrEmpty(projectNamePath))
						{
							filePath = Path.Combine(projectsFolderPath, projectNamePath, "Reports");
							if (!Directory.Exists(filePath))
							{
								// in case the original filePath was not found, search if the project is external/single file and set the filePath from the corresponding folder path
								filePath = SetExternalProjectPath(projectInfoNode, filePath);
							}
						}
					}
					else
					{
						filePath = SetExternalProjectPath(projectInfoNode, filePath);
					}
				}

				ReportsFolderPath = filePath;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetProjectFilePath method: {ex.Message}\n {ex.StackTrace}");
			}

			return filePath;
		}

		// Check if the report file exists
		private bool ReportFileExist(string reportFolderPath)
		{
			try
			{
				if (string.IsNullOrEmpty(reportFolderPath)) return false;
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

		// Configure the details used in the exported report
		private void ConfigureReportDetails(ProjectDetails project, ProjectInfo projectInfo, XmlNode report, XmlDocument doc)
		{
			try
			{
				if (report?.Attributes == null || !report.Attributes["TaskTemplateId"].Value.Equals("Sdl.ProjectApi.AutomaticTasks.Analysis")) return;
				var reportLangDirectionId = report.Attributes["LanguageDirectionGuid"].Value;
				var languageDirectionsNode = doc.SelectNodes("Project/LanguageDirections/LanguageDirection");
				if (languageDirectionsNode == null) return;
				foreach (XmlNode langDirNode in languageDirectionsNode)
				{
					if (langDirNode?.Attributes == null) continue;
					var fileLangGuid = langDirNode.Attributes["Guid"]?.Value;
					if (!reportLangDirectionId.Equals(fileLangGuid)) continue;

					var reportPath = Path.Combine(project.ProjectFolderPath, report.Attributes["PhysicalPath"].Value);
					if (!string.IsNullOrEmpty(project.ReportsFolderPath))
					{
						reportPath = Path.Combine(project.ReportsFolderPath, Path.GetFileName(report.Attributes["PhysicalPath"].Value));
					}

					SetLanguageAnalysisReportPaths(projectInfo, project, langDirNode, reportPath);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ConfigureReportDetails method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		// set the paths for the language analysis reports
		private void SetLanguageAnalysisReportPaths(ProjectInfo projectInfo, ProjectDetails projectDetails, XmlNode langDirNode, string reportPath)
		{
			try
			{
				if (projectDetails != null && projectDetails.LanguageAnalysisReportPaths == null)
				{
					projectDetails.LanguageAnalysisReportPaths = new Dictionary<string, string>();
				}

				if (langDirNode?.Attributes == null) return;
				var targetLangCode = langDirNode?.Attributes["TargetLanguageCode"].Value;
				var language = projectInfo?.TargetLanguages?.FirstOrDefault(n => n.IsoAbbreviation.Equals(targetLangCode));

				if (language == null) return;
				if (projectDetails != null && !projectDetails.LanguageAnalysisReportPaths.ContainsKey(language.DisplayName))
				{
					projectDetails.LanguageAnalysisReportPaths.Add(language.DisplayName, reportPath);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetLanguageAnalysisReportPaths method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		// Check if the report folder exists
		private bool ReportsFolderExists(string reportsFolderPath)
		{
			return !string.IsNullOrEmpty(reportsFolderPath) && ReportFileExist(reportsFolderPath);
		}

		//  Write the report file based on the Analyse file 
		private void WriteReportFile(ProjectDetails project, OptionalInformation optionalInformation, KeyValuePair<string,bool> languageReport, bool isChecked)
		{
			try
			{
				var streamPath = Path.Combine($"{project.ReportPath}{Path.DirectorySeparatorChar}", $"{project.ProjectName}_{languageReport.Key}.csv");
				using (var sw = new StreamWriter(streamPath))
				{
					if (project.LanguageAnalysisReportPaths == null) return;
					var analyseReportPath = project.LanguageAnalysisReportPaths.FirstOrDefault(l => l.Key.Equals(languageReport.Key));
					if (!analyseReportPath.Equals(new KeyValuePair<string, string>()))
					{
						var report = new StudioAnalysisReport(analyseReportPath.Value);
						sw.Write(report.ToCsv(isChecked, optionalInformation));
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"WriteReportFile method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		// Set the external's project path
		private string SetExternalProjectPath(XmlNode projectInfoNode, string filePath)
		{
			try
			{
				// is external or single file project
				var reportsPath = Path.GetDirectoryName(filePath);
				if (!string.IsNullOrEmpty(reportsPath))
				{
					filePath = Path.Combine(reportsPath, "Reports");
					if (!Directory.Exists(filePath))
					{
						// get the single file project Reports folder's path
						var directoryName = Path.GetDirectoryName(filePath);
						if (!string.IsNullOrEmpty(directoryName) && projectInfoNode?.Attributes != null)
						{
							var projectName = Path.GetFileNameWithoutExtension(projectInfoNode.Attributes["ProjectFilePath"]?.Value);
							filePath = Path.Combine(directoryName, $"{projectName}.ProjectFiles", "Reports");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetExternalProjectPath method: {ex.Message}\n {ex.StackTrace}");
			}
			return filePath;
		}
	}
}
