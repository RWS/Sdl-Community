using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NLog;
using Sdl.Community.ExportAnalysisReports.Interfaces;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class ReportService : IReportService
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IMessageBoxService _messageBoxService;
		private readonly IProjectService _projectService;
		private readonly SettingsService _settingsService;
		private string _reportFile;
		private IEnumerable<AnalyzedFile> _analyzedFiles;

		public ReportService(IMessageBoxService messageBoxService, IProjectService projectService, SettingsService settingsService)
		{
			_messageBoxService = messageBoxService;
			_projectService = projectService;
			_settingsService = settingsService;
		}


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
				var areCheckedLanguages = projectsToBeExported.Any(p => p.ProjectLanguages.Any(l => l.Value));
				if (areCheckedLanguages || projectsToBeExported.Count > 0)
				{
					foreach (var project in projectsToBeExported)
					{
						// check which languages to export
						if (project.ProjectLanguages == null) continue;
						var checkedLanguages = project.ProjectLanguages.Where(l => l.Value).ToList();

						foreach (var languageReport in checkedLanguages)
						{
							var reportPath = project.LanguageAnalysisReportPaths.FirstOrDefault(l => l.Key.Equals(languageReport.Key));
							if (!string.IsNullOrEmpty(reportPath.Value) && File.Exists(reportPath.Value))
							{
								project.ReportPath = reportOutputPath;
								WriteReportFile(project, optionalInformation, languageReport, isChecked);
							}
						}
					}
					return true;
				}
				_messageBoxService.ShowInformationMessage(PluginResources.SelectLanguage_Export_Message, PluginResources.ExportResult_Label);
				return false;
			}
			catch (Exception exception)
			{
				_logger.Error($"GenerateReport method: {exception.Message}\n {exception.StackTrace}");
				throw;
			}
		}

		/// <summary>
		/// Check if the exported path is the same
		/// </summary>
		/// <param name="reportOutputPath"></param>
		/// <returns></returns>
		public bool IsSameReportPath(string reportOutputPath)
		{
			var jsonReportPath = _settingsService.GetSettings().ExportPath;
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
				_logger.Error($"ReportFolderExist method: {ex.Message}\n {ex.StackTrace}");
			}

			return false;
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

				var projectInfo = _projectService.GetProjectInfo(project.ProjectPath);

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
							ConfigureReportDetails(project, projectInfo, (XmlNode)reportNode, doc);
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"LoadReports method: {ex.Message}\n {ex.StackTrace}");
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
					var projectDetails = _projectService.GetExternalProjectDetails(projectPath, reportFolderPath);

					SetReportInformation(projectDetails);
					externalProjInfoList.Add(projectDetails);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"SetExternalProjectReportInfo method: {ex.Message}\n {ex.StackTrace}");
			}

			return externalProjInfoList;
		}

		public void PrepareAnalysisReport(string pathToXmlReport)
		{
			try
			{
				var reportsPath = Path.GetDirectoryName(pathToXmlReport);
				if (reportsPath == null)
				{
					return;
				}

				var reportName = Path.GetFileName(pathToXmlReport);
				var directoryInfo = new DirectoryInfo(reportsPath);
				{
					var fileName = Path.GetFileNameWithoutExtension(reportName);
					var fileInfo = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault(n =>
							n.Name.StartsWith(fileName, StringComparison.CurrentCultureIgnoreCase) &&
							n.Name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase));
					_reportFile = fileInfo != null ? fileInfo.FullName : pathToXmlReport;

					if (!File.Exists(_reportFile))
					{
						_messageBoxService.ShowWarningMessage(string.Format(PluginResources.ReportNotFound_Message, _reportFile), string.Empty);
					}

					var langPairCode = GetLanguagePairCode(fileName);

					var xDoc = XDocument.Load(_reportFile);
					_analyzedFiles = from f in xDoc.Root.Descendants("file")
									 select new AnalyzedFile($"{langPairCode}_{f.Attribute("name").Value}", f.Attribute("guid").Value)
									 {
										 Results = from r in f.Element("analyse").Elements()
												   select new BandResult(r.Name.LocalName)
												   {
													   Segments = r.Attribute("segments") != null ? int.Parse(r.Attribute("segments").Value) : 0,
													   Words = r.Attribute("words") != null ? int.Parse(r.Attribute("words").Value) : 0,
													   Characters = r.Attribute("characters") != null ? int.Parse(r.Attribute("characters").Value) : 0,
													   Placeables = r.Attribute("placeables") != null ? int.Parse(r.Attribute("placeables").Value) : 0,
													   Tags = r.Attribute("tags") != null ? int.Parse(r.Attribute("tags").Value) : 0,
													   Min = r.Attribute("min") != null ? int.Parse(r.Attribute("min").Value) : 0,
													   Max = r.Attribute("max") != null ? int.Parse(r.Attribute("max").Value) : 0,
													   FullRecallWords = r.Attribute("fullRecallWords") != null ? int.Parse(r.Attribute("fullRecallWords").Value) : 0,
												   }
									 };

					if (!_analyzedFiles.Any())
					{
						_messageBoxService.ShowWarningMessage(PluginResources.NoAnalyzeFiles_Message, string.Empty);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"PrepareAnalysisReport method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		public string GetCsvContent(bool includeHeader, OptionalInformation aditionalHeaders)
		{
			try
			{
				var csvSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
				var csvHeader = GetCsvHeaderRow(csvSeparator, _analyzedFiles?.FirstOrDefault()?.Fuzzies, aditionalHeaders);
				var sb = new StringBuilder();

				if (includeHeader)
				{
					sb.Append(csvHeader).Append(Environment.NewLine);
				}

				if (_analyzedFiles != null)
				{
					foreach (var file in _analyzedFiles)
					{
						WriteCsvInformation(sb, aditionalHeaders, file, csvSeparator);
					}
				}
				return sb.ToString();
			}
			catch (Exception ex)
			{
				_logger.Error($"GetCsvContent method: {ex.Message}\n {ex.StackTrace}");
			}
			return string.Empty;
		}

		// Write to string builder the information used in csv file
		private void WriteCsvInformation(StringBuilder sb, OptionalInformation optionalInformation, AnalyzedFile file, string csvSeparator)
		{
			sb.Append(file.FileName).Append(csvSeparator).Append(file.Repeated.Words).Append(csvSeparator);

			if (optionalInformation.IncludeLocked)
			{
				sb.Append(file.Locked.Words).Append(csvSeparator);
			}
			if (optionalInformation.IncludePerfectMatch)
			{
				sb.Append(file.Perfect.Words).Append(csvSeparator);
			}
			if (optionalInformation.IncludeContextMatch)
			{
				sb.Append(file.InContextExact.Words).Append(csvSeparator);
			}
			if (optionalInformation.IncludeCrossRep)
			{
				sb.Append(file.CrossRep.Words).Append(csvSeparator);
			}

			// the 100% match is actually a sum of Exact, Perfect and InContextExact matches
			sb.Append((file.Exact.Words + file.Perfect.Words + file.InContextExact.Words).ToString()).Append(csvSeparator);

			foreach (var fuzzy in file.Fuzzies.OrderByDescending(fz => fz.Max))
			{
				sb.Append(fuzzy.Words).Append(csvSeparator);

				var internalFuzzy = file.InternalFuzzy(fuzzy.Min, fuzzy.Max);
				sb.Append(internalFuzzy != null ? internalFuzzy.Words : 0).Append(csvSeparator);
				if (optionalInformation.IncludeInternalFuzzies)
				{
					sb.Append(internalFuzzy != null ? internalFuzzy.FullRecallWords : 0).Append(csvSeparator);
				}
			}

			if (optionalInformation.IncludeAdaptiveBaseline)
			{
				sb.Append(file.NewBaseline?.FullRecallWords).Append(csvSeparator);
			}
			if (optionalInformation.IncludeAdaptiveLearnings)
			{
				sb.Append(file.NewLearnings?.FullRecallWords).Append(csvSeparator);
			}

			sb.Append(file.Untranslated.Words).Append(csvSeparator).Append(file.Total.Words).Append(csvSeparator).Append(Environment.NewLine);
		}

		private string GetLanguagePairCode(string fileName)
		{
			var langPairCodes = !string.IsNullOrEmpty(fileName) ? fileName.Split(' ') : new string[3];
			return langPairCodes.Count() == 3 ? langPairCodes[2] : string.Empty;
		}

		private string GetCsvHeaderRow(string separator, IEnumerable<BandResult> fuzzies, OptionalInformation aditionalHeaders)
		{
			try
			{
				var headerColumns = new List<string> { "\"Filename\"", "\"Repetitions\"", };

				if (aditionalHeaders.IncludeLocked)
				{
					headerColumns.Add("\"Locked\"");
				}

				if (aditionalHeaders.IncludePerfectMatch)
				{
					headerColumns.Add("\"Perfect Match\"");
				}

				if (aditionalHeaders.IncludeContextMatch)
				{
					headerColumns.Add("\"Context Match\"");
				}

				if (aditionalHeaders.IncludeCrossRep)
				{
					headerColumns.Add("\"Cross File Repetitions\"");
				}

				headerColumns.Add("\"100% (TM)\"");
				fuzzies.OrderByDescending(br => br.Max).ToList().ForEach(br =>
				{
					headerColumns.Add(string.Format("\"{0}% - {1}% (TM)\"", br.Max, br.Min));
					headerColumns.Add(string.Format("\"{0}% - {1}% (AP)\"", br.Max, br.Min));
					if (aditionalHeaders.IncludeInternalFuzzies)
					{
						headerColumns.Add(string.Format("\"{0}% - {1}% (Internal)\"", br.Max, br.Min));
					}
				});

				if (aditionalHeaders.IncludeAdaptiveBaseline)
				{
					headerColumns.Add("\"AdaptiveMT Baseline\"");
				}
				if (aditionalHeaders.IncludeAdaptiveLearnings)
				{
					headerColumns.Add("\"AdaptiveMT with Learnings\"");
				}
				headerColumns.Add("\"New\"");
				headerColumns.Add("\"Total Words\"");

				return headerColumns.Aggregate((res, next) => res + separator + next);
			}
			catch (Exception ex)
			{
				_logger.Error($"GetCsvHeaderRow method: {ex.Message}\n {ex.StackTrace}");
			}
			return string.Empty;
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
			}
			catch (Exception ex)
			{
				_logger.Error($"SetProjectFilePath method: {ex.Message}\n {ex.StackTrace}");
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
					return files.Any(file => IsAnalyzeFileReport(file));
				}

				if (!string.IsNullOrEmpty(fileName) && fileName.Contains("ProjectFiles") && Directory.Exists(reportFolderPath))
				{
					fileName = Path.GetFileNameWithoutExtension(fileName);
					return !string.IsNullOrEmpty(fileName);
				}

				return false;
			}
			catch (Exception ex)
			{
				_logger.Error($"ReportFileExist method: {ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}

		private bool IsAnalyzeFileReport(string filePath)
		{
			try
			{
				if (!filePath.EndsWith(".xml"))
				{
					return false;
				}

                var firstLine = File.ReadLines(filePath).FirstOrDefault();

                if (firstLine != null && firstLine.Trim().StartsWith("<task name=\"analyse\">"))
                {
                    return true;
                }

				return false;
            }
			catch (Exception _)
			{
				return false;
			}
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

					if (!File.Exists(reportPath))
					{
						continue;
					}

					SetLanguageAnalysisReportPaths(projectInfo, project, langDirNode, reportPath);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"ConfigureReportDetails method: {ex.Message}\n {ex.StackTrace}");
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
				var language = projectInfo?.TargetLanguages?.FirstOrDefault(n => n.IsoAbbreviation.ToLower().Equals(targetLangCode.ToLower()));

				if (language == null) return;
				if (projectDetails != null && !projectDetails.LanguageAnalysisReportPaths.ContainsKey(language.DisplayName))
				{
					projectDetails.LanguageAnalysisReportPaths.Add(language.DisplayName, reportPath);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"SetLanguageAnalysisReportPaths method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		// Check if the report folder exists
		private bool ReportsFolderExists(string reportsFolderPath)
		{
			return !string.IsNullOrEmpty(reportsFolderPath) && ReportFileExist(reportsFolderPath);
		}

		//  Write the report file based on the Analyse file 
		private void WriteReportFile(ProjectDetails project, OptionalInformation optionalInformation, KeyValuePair<string, bool> languageReport, bool isChecked)
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
						PrepareAnalysisReport(analyseReportPath.Value);
						sw.Write(GetCsvContent(isChecked, optionalInformation));
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"WriteReportFile method: {ex.Message}\n {ex.StackTrace}");
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
				_logger.Error($"SetExternalProjectPath method: {ex.Message}\n {ex.StackTrace}");
			}
			return filePath;
		}
	}
}
