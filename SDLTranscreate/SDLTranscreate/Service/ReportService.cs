using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Reports.Viewer.Api.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Model;
using Trados.Transcreate.Service.ProgressDialog;
using AnalysisBand = Trados.Transcreate.Model.AnalysisBand;
using ConfirmationStatistics = Trados.Transcreate.Model.ConfirmationStatistics;
using File = Trados.Transcreate.FileTypeSupport.XLIFF.Model.File;
using PathInfo = Trados.Transcreate.Common.PathInfo;
using ProjectFile = Trados.Transcreate.Model.ProjectFile;

namespace Trados.Transcreate.Service
{
	public class ReportService
	{
		private readonly PathInfo _pathInfo;
		private readonly ProjectAutomationService _projectAutomationService;
		private readonly SegmentBuilder _segmentBuilder;

		public ReportService(PathInfo pathInfo, ProjectAutomationService projectAutomationService, SegmentBuilder segmentBuilder)
		{
			_pathInfo = pathInfo;
			_projectAutomationService = projectAutomationService;
			_segmentBuilder = segmentBuilder;
		}

		public List<Report> CreateFinalReport(Interfaces.IProject project, FileBasedProject studioProject, List<ProjectFile> selectedFiles, out string workingPathOut)
		{
			var reports = new List<Report>();
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};
			var reportName = "Trados Transcreate Report";

			var studioProjectInfo = studioProject.GetProjectInfo();
			var dateTimeStamp = DateTime.UtcNow;
			var dataTimeStampToString = DateTimeStampToString(dateTimeStamp);
			var workflowPath = GetPath(studioProjectInfo.LocalProjectFolder, "Workflow");
			var actionPath = GetPath(workflowPath, "Report");
			var workingPath = GetPath(actionPath, dataTimeStampToString);

			var exportOptions = new ExportOptions();
			exportOptions.IncludeBackTranslations = true;
			exportOptions.IncludeTranslations = true;
			exportOptions.CopySourceToTarget = false;

			var analysisBands = _projectAutomationService.GetAnalysisBands(studioProject);

			var progressSettings = new ProgressDialogSettings(ApplicationInstance.GetActiveForm(), true, true, false);
			var result = ProgressDialog.ProgressDialog.Execute("Create Transcreate Reports", () =>
			{
				var sdlxliffReader = new SdlxliffReader(_segmentBuilder, exportOptions, analysisBands);
				decimal maximum = project.ProjectFiles.Count;
				decimal current = 0;
				foreach (var targetLanguage in project.TargetLanguages)
				{
					var projectFiles = project.ProjectFiles.Where(a =>
						string.Compare(a.TargetLanguage, targetLanguage.CultureInfo.Name, StringComparison.CurrentCultureIgnoreCase) == 0).ToList();

					var hasProjectFiles = HasProjectFiles(selectedFiles, projectFiles);
					if (!hasProjectFiles)
					{
						current += projectFiles.Count;
						var progress = current / maximum * 100;

						ProgressDialog.ProgressDialog.Current.Report((int)progress,
							string.Format("Language: {0}\r\nFile: {1}", targetLanguage.CultureInfo.DisplayName, projectFiles.FirstOrDefault().Name));

						continue;
					}

					var workingLanguageFolder = GetPath(workingPath, targetLanguage.CultureInfo.Name);
					foreach (var projectFile in projectFiles)
					{
						if (ProgressDialog.ProgressDialog.Current.CheckCancellationPending())
						{
							ProgressDialog.ProgressDialog.Current.ThrowIfCancellationPending();
						}

						current++;
						var progress = current / maximum * 100;
						ProgressDialog.ProgressDialog.Current.Report((int)progress, string.Format("Language: {0}\r\nFile: {1}", targetLanguage.CultureInfo.DisplayName, projectFile.Name));

						if (selectedFiles != null && !selectedFiles.Exists(a => a.FileId == projectFile.FileId))
						{
							continue;
						}

						var projectFilePath = Path.Combine(project.Path, projectFile.Location);
						var xliffData = sdlxliffReader.ReadFile(project.Id, projectFile.FileId, projectFilePath,
								targetLanguage.CultureInfo.Name);

						var backTranslationProject = GetBackTranslationProject(project, targetLanguage.CultureInfo.Name);
						var backTranslationFile = GetBackTranslationProjectFile(backTranslationProject, projectFile);
						var xliffDataBackTranslation = GetBackTranslationXliffData(backTranslationProject, backTranslationFile, sdlxliffReader);

						var fileName = projectFile.Name.Substring(0, projectFile.Name.LastIndexOf(".", StringComparison.Ordinal));
						var reportFile = Path.Combine(workingLanguageFolder, fileName + ".xml");

						using (var writer = XmlWriter.Create(reportFile, settings))
						{
							writer.WriteStartElement("task");
							writer.WriteAttributeString("name", reportName);
							writer.WriteAttributeString("created", dataTimeStampToString);

							writer.WriteStartElement("taskInfo");
							writer.WriteAttributeString("action", "Trados Transcreate Report");
							writer.WriteAttributeString("file", projectFile.Path + projectFile.Name);
							writer.WriteAttributeString("taskId", Guid.NewGuid().ToString());
							writer.WriteAttributeString("runAt", GetDisplayDateTime(dateTimeStamp));


							WriteReportProject(writer, "project", project);
							WriteReportProject(writer, "backProject", backTranslationProject);

							WriteReportLanguage(writer, "source", project.SourceLanguage.CultureInfo.Name);
							WriteReportLanguage(writer, "target", targetLanguage.CultureInfo.Name);

							WriteReportCustomer(writer, project);

							WriteReportTranslationProviders(writer, studioProject);

							writer.WriteEndElement(); //taskInfo


							writer.WriteStartElement("translations");
							foreach (var dataFile in xliffData.Files)
							{
								writer.WriteStartElement("version");
								writer.WriteAttributeString("type", dataFile.Original);

								var backTranslationDataFile =
										xliffDataBackTranslation?.Files.FirstOrDefault(a => a.Original == dataFile.Original);

								writer.WriteStartElement("segments");
								foreach (var transUnit in dataFile.Body.TransUnits)
								{
									var textFunction = transUnit.Contexts.FirstOrDefault(
											a => string.Compare(a.DisplayName, "Text Function", StringComparison.CurrentCultureIgnoreCase) == 0);

									foreach (var segmentPair in transUnit.SegmentPairs)
									{
										writer.WriteStartElement("segment");

										var backTranslationSegmentPair = GetBackTranslationSegmentPair(backTranslationDataFile, segmentPair);

										writer.WriteAttributeString("id", segmentPair.Id);
										writer.WriteAttributeString("textFunction", textFunction?.Description ?? string.Empty);

										writer.WriteStartElement("source");
										writer.WriteString(segmentPair.Source.ToString());
										writer.WriteEndElement(); //source

										writer.WriteStartElement("target");
										writer.WriteString(segmentPair.Target.ToString());
										writer.WriteEndElement(); //source

										writer.WriteStartElement("back");
										writer.WriteString(backTranslationSegmentPair?.Target?.ToString() ?? string.Empty);
										writer.WriteEndElement(); //backTranslation

										writer.WriteStartElement("comments");
										var comments = GetSegmentComments(segmentPair.Target, xliffData.DocInfo);
										if (comments != null)
										{
											foreach (var comment in comments)
											{
												writer.WriteStartElement("comment");
												writer.WriteAttributeString("version", comment.Version);
												writer.WriteAttributeString("author", comment.Author);
												writer.WriteAttributeString("severity", comment.Severity.ToString());
												writer.WriteAttributeString("date", GetDisplayDateTime(comment.Date));
												writer.WriteString(comment.Text ?? string.Empty);
												writer.WriteEndElement(); //comment
											}
										}

										writer.WriteEndElement(); //comments

										writer.WriteEndElement(); //segment
									}
								}
								writer.WriteEndElement(); //segments

								writer.WriteEndElement(); //version
							}
							writer.WriteEndElement(); //translations
						}

						// transform the file against an xslt
						var templatePath = GetReportTemplatePath("TranscreateFinalReport.xsl");
						var reportFilePath = CreateHtmlReportFile(reportFile, templatePath);


						var report = new Report
						{
							Name = fileName,
							Date = dateTimeStamp,
							Description = "Transcreate Report",
							Group = "Transcreate Report",
							Language = targetLanguage.CultureInfo.Name,
							Path = reportFilePath
						};
						reports.Add(report);
					}
				}

			}, progressSettings);

			workingPathOut = workingPath;

			if (result.Cancelled)
			{
				System.Windows.Forms.MessageBox.Show("Process cancelled by user.", PluginResources.Plugin_Name);
				return new List<Report>();
			}

			return reports;

		}

		private static bool HasProjectFiles(List<ProjectFile> selectedFiles, List<ProjectFile> projectFiles)
		{
			var hasProjectFiles = true;

			if (selectedFiles?.Count > 0 && projectFiles.Any())
			{
				var availableFiles = projectFiles
					.Where(projectFile => selectedFiles.Exists(a => a.FileId == projectFile.FileId)).ToList();

				if (!availableFiles.Any())
				{
					hasProjectFiles = false;
				}
			}
			else if (!projectFiles.Any())
			{
				hasProjectFiles = false;
			}

			return hasProjectFiles;
		}

		public void CreateTaskReport(TaskContext taskContext, string reportFile,
			FileBasedProject selectedProject, string targetLanguageCode)
		{
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};

			var projectFiles = taskContext.ProjectFiles.Where(a => a.Selected &&
																   string.Compare(a.TargetLanguage, targetLanguageCode,
																	   StringComparison.CurrentCultureIgnoreCase) == 0).ToList();

			var reportName = "";
			switch (taskContext.Action)
			{
				case Enumerators.Action.Convert:
					reportName = "Create Transcreate Project Report";
					break;
				case Enumerators.Action.CreateBackTranslation:
					if (taskContext.Project is BackTranslationProject backTranslationProject && backTranslationProject.IsUpdate)
					{
						reportName = "Update Back-Translation Project Report";
					}
					else
					{
						reportName = "Create Back-Translation Project Report";
					}
					break;
				case Enumerators.Action.Export:
					reportName = "Export Translations Report";
					break;
				case Enumerators.Action.Import:
					reportName = "Import Translations Report";
					break;
				case Enumerators.Action.ExportBackTranslation:
					reportName = "Export Back-Translations Report";
					break;
				case Enumerators.Action.ImportBackTranslation:
					reportName = "Import Back-Translations Report";
					break;
			}

			using (var writer = XmlWriter.Create(reportFile, settings))
			{
				writer.WriteStartElement("task");
				writer.WriteAttributeString("name", reportName);
				writer.WriteAttributeString("created", taskContext.DateTimeStampToString);

				WriteReportTaskInfo(writer, taskContext, selectedProject, targetLanguageCode);

				foreach (var projectFile in projectFiles)
				{
					WriteReportFile(writer, taskContext, projectFile);
				}

				WriteReportTotal(writer, taskContext, projectFiles);

				writer.WriteEndElement(); //task
			}
		}

		private string CreateHtmlReportFile(string xmlReportFullPath, string xsltFilePath)
		{
			var htmlReportFilePath = xmlReportFullPath + ".html";

			var xsltSetting = new XsltSettings
			{
				EnableDocumentFunction = true,
				EnableScript = true
			};

			var myXPathDoc = new XPathDocument(xmlReportFullPath);

			var myXslTrans = new XslCompiledTransform();
			myXslTrans.Load(xsltFilePath, xsltSetting, null);

			var myWriter = new XmlTextWriter(htmlReportFilePath, Encoding.UTF8);

			myXslTrans.Transform(myXPathDoc, null, myWriter);

			myWriter.Flush();
			myWriter.Close();

			return htmlReportFilePath;
		}

		public string GetReportTemplatePath(string name)
		{
			var filePath = Path.Combine(_pathInfo.SettingsFolderPath, name);
			var resourceName = "Trados.Transcreate.Resources." + name;

			WriteResourceToFile(resourceName, filePath);

			return filePath;
		}

		public void WriteResourceToFile(string resourceName, string fullFilePath)
		{
			if (System.IO.File.Exists(fullFilePath))
			{
				System.IO.File.Delete(fullFilePath);
			}

			using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			{
				using (var file = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
				{
					resource?.CopyTo(file);
				}
			}
		}

		private string GetDisplayDateTime(DateTime dateTime)
		{
			if (dateTime == DateTime.MaxValue || dateTime == DateTime.MinValue)
			{
				return string.Empty;
			}
			return dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
		}

		private List<IComment> GetSegmentComments(Segment segment, DocInfo docInfo)
		{
			if (docInfo?.Comments == null || docInfo.Comments?.Keys.Count == 0)
			{
				return null;
			}

			var comments = new List<IComment>();
			foreach (var element in segment.Elements)
			{
				if (element is ElementComment comment && comment.Type == Element.TagType.TagOpen)
				{
					if (docInfo.Comments.ContainsKey(comment.Id))
					{
						var commentList = docInfo.Comments[comment.Id];
						comments.AddRange(commentList);
					}
				}
			}

			return comments;
		}

		private Xliff GetBackTranslationXliffData(Interfaces.IProject project, ProjectFile projectFile, SdlxliffReader sdlxliffReader)
		{
			if (project != null && projectFile != null)
			{
				var backTranslationFilePath =
					Path.Combine(project.Path, projectFile.Location);

				if (System.IO.File.Exists(backTranslationFilePath))
				{
					return sdlxliffReader.ReadFile(project.Id, projectFile.FileId,
						backTranslationFilePath,
						projectFile.TargetLanguage);
				}
			}

			return null;
		}

		private static ProjectFile GetBackTranslationProjectFile(Interfaces.IProject backTranslationProject, ProjectFile projectFile)
		{
			if (backTranslationProject == null)
			{
				return null;
			}

			foreach (var translationProjectFile in backTranslationProject.ProjectFiles)
			{
				if (string.Compare(projectFile.Name, translationProjectFile.Name,
						StringComparison.CurrentCultureIgnoreCase) == 0 &&
					string.Compare(projectFile.Path, translationProjectFile.Path,
						StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return translationProjectFile;
				}
			}

			return null;
		}

		private Interfaces.IProject GetBackTranslationProject(Interfaces.IProject project, string targetLanguage)
		{
			foreach (var backTranslationProject in project.BackTranslationProjects)
			{
				if (string.Compare(backTranslationProject.SourceLanguage.CultureInfo.Name,
					targetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return backTranslationProject;
				}
			}

			return null;
		}

		private static SegmentPair GetBackTranslationSegmentPair(File backTranslationDataFile, SegmentPair segmentPair)
		{
			if (backTranslationDataFile?.Body == null)
			{
				return null;
			}

			foreach (var transUnit in backTranslationDataFile.Body.TransUnits)
			{
				foreach (var pair in transUnit.SegmentPairs)
				{
					if (pair.Id == segmentPair.Id)
					{
						return pair;
					}
				}
			}

			return null;
		}



		private void WriteReportTotal(XmlWriter writer, TaskContext taskContext, IReadOnlyCollection<ProjectFile> projectFiles)
		{
			writer.WriteStartElement("batchTotal");

			var totalTranslationOriginStatistics = GetTotalTranslationOriginStatistics(projectFiles, taskContext.Action);
			WriteAnalysisXml(writer, totalTranslationOriginStatistics?.WordCounts, taskContext.AnalysisBands, taskContext.Action);

			var totalConfirmationStatistics = GetTotalConfirmationStatistics(projectFiles, taskContext.Action);
			WriteConfirmationXml(writer, totalConfirmationStatistics?.WordCounts, taskContext.Action);

			writer.WriteEndElement(); //batchTotal
		}

		private void WriteReportFile(XmlWriter writer, TaskContext taskContext, ProjectFile projectFile)
		{
			writer.WriteStartElement("file");
			writer.WriteAttributeString("name", Path.Combine(projectFile.Path, projectFile.Name));
			writer.WriteAttributeString("guid", projectFile.FileId);

			WriteAnalysisXml(writer, projectFile.TranslationOriginStatistics?.WordCounts, taskContext.AnalysisBands, taskContext.Action);
			WriteConfirmationXml(writer, projectFile.ConfirmationStatistics?.WordCounts, taskContext.Action);

			writer.WriteEndElement(); //file
		}

		private void WriteReportTaskInfo(XmlWriter writer, TaskContext taskContext, IProject fileBasedProject, string languageCode)
		{
			writer.WriteStartElement("taskInfo");
			writer.WriteAttributeString("action", taskContext.Action.ToString());
			writer.WriteAttributeString("workflow", taskContext.WorkFlow.ToString());
			writer.WriteAttributeString("taskId", Guid.NewGuid().ToString());
			writer.WriteAttributeString("runAt", taskContext.DateTimeStamp.ToShortDateString() + " " + taskContext.DateTimeStamp.ToShortTimeString());

			WriteReportProject(writer, "project", taskContext.Project);

			WriteReportLanguage(writer, "language", languageCode);

			WriteReportCustomer(writer, taskContext.Project);

			WriteReportTranslationProviders(writer, fileBasedProject);

			WriteReportSettings(writer, taskContext);

			writer.WriteEndElement(); //taskInfo
		}

		private static void WriteReportTranslationProviders(XmlWriter writer, IProject fileBasedProject)
		{
			//var cultureInfo = new CultureInfo(languageDirection.Key.TargetLanguageCode);
			//var language = new Language(cultureInfo);
			var config = fileBasedProject.GetTranslationProviderConfiguration();
			foreach (var cascadeEntry in config.Entries)
			{
				if (!cascadeEntry.MainTranslationProvider.Enabled)
				{
					continue;
				}

				var scheme = cascadeEntry.MainTranslationProvider.Uri.Scheme;
				var segments = cascadeEntry.MainTranslationProvider.Uri.Segments;
				var name = scheme + "://";
				if (segments.Length >= 0)
				{
					name += segments[segments.Length - 1];
				}

				writer.WriteStartElement("tm");
				writer.WriteAttributeString("name", name);
				writer.WriteEndElement(); //tm
			}
		}

		private static void WriteReportLanguage(XmlWriter writer, string elementName, string languageCode)
		{
			writer.WriteStartElement(elementName);
			writer.WriteAttributeString("id", languageCode);
			writer.WriteAttributeString("name", new CultureInfo(languageCode).DisplayName);
			writer.WriteEndElement(); //language
		}

		private static void WriteReportCustomer(XmlWriter writer, Interfaces.IProject project)
		{
			if (!string.IsNullOrEmpty(project.Customer?.Name))
			{
				writer.WriteStartElement("customer");
				writer.WriteAttributeString("name", project.Customer.Name);
				writer.WriteAttributeString("email", project.Customer.Email);
				writer.WriteEndElement(); //customer												  
			}
		}

		private void WriteReportProject(XmlWriter writer, string elementName, Interfaces.IProject project)
		{
			if (project == null)
			{
				return;
			}

			writer.WriteStartElement(elementName);
			writer.WriteAttributeString("name", project.Name);
			writer.WriteAttributeString("number", project.Id);

			var minValue = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
			var maxValue = DateTime.MaxValue.ToString(CultureInfo.InvariantCulture);

			var utcMinValue = DateTime.MinValue.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
			var utcMaxValue = DateTime.MaxValue.ToUniversalTime().ToString(CultureInfo.InvariantCulture);

			var dueDateValue = project.DueDate.ToString(CultureInfo.InvariantCulture);

			if (dueDateValue != minValue && dueDateValue != maxValue &&
				dueDateValue != utcMinValue && dueDateValue != utcMaxValue)
			{
				writer.WriteAttributeString("dueDate", GetDisplayDateTime(project.DueDate));
			}

			writer.WriteEndElement(); //project
		}

		private void WriteReportSettings(XmlWriter writer, TaskContext taskContext)
		{
			writer.WriteStartElement("settings");
			if (taskContext.Action == Enumerators.Action.Export || taskContext.Action == Enumerators.Action.ExportBackTranslation)
			{
				writer.WriteAttributeString("xliffSupport", taskContext.ExportOptions.XliffSupport.ToString());
				writer.WriteAttributeString("includeTranslations", taskContext.ExportOptions.IncludeTranslations.ToString());
				writer.WriteAttributeString("copySourceToTarget", taskContext.ExportOptions.CopySourceToTarget.ToString());
				writer.WriteAttributeString("excludeFilterItems", GetFitlerItemsString(taskContext.ExportOptions.ExcludeFilterIds));
			}
			else if (taskContext.Action == Enumerators.Action.Import || taskContext.Action == Enumerators.Action.ImportBackTranslation)
			{
				writer.WriteAttributeString("overwriteTranslations", taskContext.ImportOptions.OverwriteTranslations.ToString());
				writer.WriteAttributeString("originSystem", taskContext.ImportOptions.OriginSystem);
				writer.WriteAttributeString("statusTranslationUpdatedId", GetSegmentStatus(taskContext.ImportOptions.StatusTranslationUpdatedId));
				writer.WriteAttributeString("statusTranslationNotUpdatedId", GetSegmentStatus(taskContext.ImportOptions.StatusTranslationNotUpdatedId));
				writer.WriteAttributeString("statusSegmentNotImportedId", GetSegmentStatus(taskContext.ImportOptions.StatusSegmentNotImportedId));
				writer.WriteAttributeString("excludeFilterItems", GetFitlerItemsString(taskContext.ImportOptions.ExcludeFilterIds));
			}
			else if (taskContext.Action == Enumerators.Action.Convert)
			{
				writer.WriteAttributeString("maxAlternativeTranslations", taskContext.ConvertOptions.MaxAlternativeTranslations.ToString());
				writer.WriteAttributeString("closeProjectOnComplete", taskContext.ConvertOptions.CloseProjectOnComplete.ToString());
			}

			writer.WriteEndElement(); //settings
		}


		private string GetSegmentStatus(string id)
		{
			var confirmationStatuses = Enumerators.GetConfirmationStatuses();
			var status = confirmationStatuses.FirstOrDefault(a => a.Id == id);
			return status != null ? status.Name : "Don't Change";
		}

		private void WriteConfirmationXml(XmlWriter writer, WordCounts wordCounts, Enumerators.Action action)
		{
			writer.WriteStartElement("confirmation");

			WriteConfirmationWordCountStatistics(writer, wordCounts?.Processed, "processed");
			WriteConfirmationWordCountStatistics(writer, wordCounts?.Excluded, "excluded");
			if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
			{
				WriteConfirmationWordCountStatistics(writer, wordCounts?.NotProcessed, "notProcessed");
			}
			WriteConfirmationWordCountStatistics(writer, wordCounts?.Total, "total");

			writer.WriteEndElement(); //confirmation
		}

		private void WriteAnalysisXml(XmlWriter writer, WordCounts wordCounts, IReadOnlyCollection<AnalysisBand> analysisBands, Enumerators.Action action)
		{
			writer.WriteStartElement("analysis");

			WriteAnalysisWordCountStatistics(writer, wordCounts?.Processed, analysisBands, "processed");
			WriteAnalysisWordCountStatistics(writer, wordCounts?.Excluded, analysisBands, "excluded");
			if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
			{
				WriteAnalysisWordCountStatistics(writer, wordCounts?.NotProcessed, analysisBands, "notProcessed");
			}
			WriteAnalysisWordCountStatistics(writer, wordCounts?.Total, analysisBands, "total");

			writer.WriteEndElement(); //analysis
		}

		private TranslationOriginStatistics GetTotalTranslationOriginStatistics(IEnumerable<ProjectFile> projectFiles, Enumerators.Action action)
		{
			var statistics = new TranslationOriginStatistics();

			foreach (var projectFile in projectFiles)
			{
				if (projectFile.TranslationOriginStatistics?.WordCounts != null)
				{
					foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.Processed)
					{
						var totalWordCount = statistics.WordCounts.Processed.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Processed.Add(wordCount);
						}
					}

					foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.Excluded)
					{
						var totalWordCount = statistics.WordCounts.Excluded.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Excluded.Add(wordCount);
						}
					}

					if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
					{
						foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.NotProcessed)
						{
							var totalWordCount = statistics.WordCounts.NotProcessed.FirstOrDefault(a =>
								a.Category == wordCount.Category);
							if (totalWordCount != null)
							{
								UpdateTotalWordCount(wordCount, totalWordCount);
							}
							else
							{
								statistics.WordCounts.NotProcessed.Add(wordCount);
							}
						}
					}

					foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.Total)
					{
						var totalWordCount = statistics.WordCounts.Total.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Total.Add(wordCount);
						}
					}
				}
			}

			return statistics;
		}

		private ConfirmationStatistics GetTotalConfirmationStatistics(IEnumerable<ProjectFile> projectFiles, Enumerators.Action action)
		{
			var statistics = new ConfirmationStatistics();

			foreach (var projectFile in projectFiles)
			{
				if (projectFile.ConfirmationStatistics?.WordCounts != null)
				{
					foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.Processed)
					{
						var totalWordCount = statistics.WordCounts.Processed.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Processed.Add(wordCount);
						}
					}

					foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.Excluded)
					{
						var totalWordCount = statistics.WordCounts.Excluded.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Excluded.Add(wordCount);
						}
					}

					if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
					{
						foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.NotProcessed)
						{
							var totalWordCount = statistics.WordCounts.NotProcessed.FirstOrDefault(a =>
								a.Category == wordCount.Category);
							if (totalWordCount != null)
							{
								UpdateTotalWordCount(wordCount, totalWordCount);
							}
							else
							{
								statistics.WordCounts.NotProcessed.Add(wordCount);
							}
						}
					}

					foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.Total)
					{
						var totalWordCount = statistics.WordCounts.Total.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Total.Add(wordCount);
						}
					}
				}
			}

			return statistics;
		}

		private string GetFitlerItemsString(IEnumerable<string> ids)
		{
			var allFilterItems = Enumerators.GetFilterItems();
			var filterItems = Enumerators.GetFilterItems(allFilterItems, ids);
			var items = string.Empty;
			foreach (var filterItem in filterItems)
			{
				items += (string.IsNullOrEmpty(items) ? string.Empty : ", ") +
						 filterItem.Name;
			}

			if (string.IsNullOrEmpty(items))
			{
				items = "[none]";
			}

			return items;
		}

		private void WriteAnalysisWordCountStatistics(XmlWriter writer,
		IReadOnlyCollection<WordCount> wordCounts, IEnumerable<AnalysisBand> analysisBands, string name)
		{
			writer.WriteStartElement(name);

			var totalWordCount = new WordCount
			{
				Category = "Total"
			};

			var perfectMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.PM.ToString()) ?? new WordCount { Category = Enumerators.MatchType.PM.ToString() };
			WriteWordCount(writer, perfectMatch, "perfect");
			UpdateTotalWordCount(perfectMatch, totalWordCount);

			var contextMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.CM.ToString()) ?? new WordCount { Category = Enumerators.MatchType.CM.ToString() };
			WriteWordCount(writer, contextMatch, "context");
			UpdateTotalWordCount(contextMatch, totalWordCount);

			var repetitionMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.Repetition.ToString()) ?? new WordCount { Category = Enumerators.MatchType.Repetition.ToString() };
			WriteWordCount(writer, repetitionMatch, "repetition");
			UpdateTotalWordCount(repetitionMatch, totalWordCount);

			var exactMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.Exact.ToString()) ?? new WordCount { Category = Enumerators.MatchType.Exact.ToString() };
			WriteWordCount(writer, exactMatch, "exact");
			UpdateTotalWordCount(exactMatch, totalWordCount);

			foreach (var analysisBand in analysisBands)
			{
				var fuzzyMatchKey = string.Format("{0} {1} - {2}", Enumerators.MatchType.Fuzzy.ToString(),
					analysisBand.MinimumMatchValue + "%", analysisBand.MaximumMatchValue + "%");

				var fuzzyMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == fuzzyMatchKey) ?? new WordCount { Category = fuzzyMatchKey };

				WriteWordCount(writer, fuzzyMatch, "fuzzy",
					analysisBand.MinimumMatchValue, analysisBand.MaximumMatchValue);

				UpdateTotalWordCount(fuzzyMatch, totalWordCount);
			}

			var newMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.New.ToString()) ?? new WordCount { Category = Enumerators.MatchType.New.ToString() };
			WriteWordCount(writer, newMatch, "new");
			UpdateTotalWordCount(newMatch, totalWordCount);

			var nmtMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.NMT.ToString()) ?? new WordCount { Category = Enumerators.MatchType.NMT.ToString() };
			WriteWordCount(writer, nmtMatch, "nmt");
			UpdateTotalWordCount(nmtMatch, totalWordCount);

			var amtMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.AMT.ToString()) ?? new WordCount { Category = Enumerators.MatchType.AMT.ToString() };
			WriteWordCount(writer, amtMatch, "amt");
			UpdateTotalWordCount(amtMatch, totalWordCount);

			var mtMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.MT.ToString()) ?? new WordCount { Category = Enumerators.MatchType.MT.ToString() };
			WriteWordCount(writer, mtMatch, "mt");
			UpdateTotalWordCount(mtMatch, totalWordCount);

			WriteWordCount(writer, totalWordCount, "total");

			writer.WriteEndElement();
		}

		private void WriteConfirmationWordCountStatistics(XmlWriter writer, IReadOnlyCollection<WordCount> wordCounts, string name)
		{
			writer.WriteStartElement(name);

			var totalWordCount = new WordCount
			{
				Category = "Total"
			};

			var approvedSignOff = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.ApprovedSignOff.ToString()) ?? new WordCount { Category = ConfirmationLevel.ApprovedSignOff.ToString() };
			WriteWordCount(writer, approvedSignOff, "approvedSignOff");
			UpdateTotalWordCount(approvedSignOff, totalWordCount);

			var rejectedSignOff = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.RejectedSignOff.ToString()) ?? new WordCount { Category = ConfirmationLevel.RejectedSignOff.ToString() };
			WriteWordCount(writer, rejectedSignOff, "rejectedSignOff");
			UpdateTotalWordCount(rejectedSignOff, totalWordCount);

			var approvedTranslation = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.ApprovedTranslation.ToString()) ?? new WordCount { Category = ConfirmationLevel.ApprovedTranslation.ToString() };
			WriteWordCount(writer, approvedTranslation, "approvedTranslation");
			UpdateTotalWordCount(approvedTranslation, totalWordCount);

			var rejectedTranslation = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.RejectedTranslation.ToString()) ?? new WordCount { Category = ConfirmationLevel.RejectedTranslation.ToString() };
			WriteWordCount(writer, rejectedTranslation, "rejectedTranslation");
			UpdateTotalWordCount(rejectedTranslation, totalWordCount);

			var translated = wordCounts?.FirstOrDefault(a =>
				 a.Category == ConfirmationLevel.Translated.ToString()) ?? new WordCount { Category = ConfirmationLevel.Translated.ToString() };
			WriteWordCount(writer, translated, "translated");
			UpdateTotalWordCount(translated, totalWordCount);

			var draft = wordCounts?.FirstOrDefault(a =>
				 a.Category == ConfirmationLevel.Draft.ToString()) ?? new WordCount { Category = ConfirmationLevel.Draft.ToString() };
			WriteWordCount(writer, draft, "draft");
			UpdateTotalWordCount(draft, totalWordCount);

			var unspecified = wordCounts?.FirstOrDefault(a =>
				 a.Category == ConfirmationLevel.Unspecified.ToString()) ?? new WordCount { Category = ConfirmationLevel.Unspecified.ToString() };
			WriteWordCount(writer, unspecified, "unspecified");
			UpdateTotalWordCount(unspecified, totalWordCount);

			WriteWordCount(writer, totalWordCount, "total");

			writer.WriteEndElement();
		}

		private static void WriteWordCount(XmlWriter writer, WordCount wordCount, string name)
		{
			writer.WriteStartElement(name);
			writer.WriteAttributeString("segments", wordCount.Segments.ToString());
			writer.WriteAttributeString("words", wordCount.Words.ToString());
			writer.WriteAttributeString("characters", wordCount.Characters.ToString());
			writer.WriteAttributeString("placeables", wordCount.Placeables.ToString());
			writer.WriteAttributeString("tags", wordCount.Tags.ToString());
			writer.WriteEndElement(); //name			
		}

		private void UpdateTotalWordCount(WordCount wordCount, WordCount totalWordCount)
		{
			totalWordCount.Segments += wordCount.Segments;
			totalWordCount.Words += wordCount.Words;
			totalWordCount.Characters += wordCount.Characters;
			totalWordCount.Placeables += wordCount.Placeables;
			totalWordCount.Tags += wordCount.Tags;
		}

		private static void WriteWordCount(XmlWriter writer, WordCount wordCount, string name, int min, int max)
		{
			writer.WriteStartElement(name);
			writer.WriteAttributeString("min", min.ToString());
			writer.WriteAttributeString("max", max.ToString());
			writer.WriteAttributeString("segments", wordCount.Segments.ToString());
			writer.WriteAttributeString("words", wordCount.Words.ToString());
			writer.WriteAttributeString("characters", wordCount.Characters.ToString());
			writer.WriteAttributeString("placeables", wordCount.Placeables.ToString());
			writer.WriteAttributeString("tags", wordCount.Tags.ToString());
			writer.WriteEndElement(); //name
		}

		public string DateTimeStampToString(DateTime dt)
		{

			var value = dt.Year
						+ "" + dt.Month.ToString().PadLeft(2, '0')
						+ "" + dt.Day.ToString().PadLeft(2, '0')
						+ "" + dt.Hour.ToString().PadLeft(2, '0')
						+ "" + dt.Minute.ToString().PadLeft(2, '0')
						+ "" + dt.Second.ToString().PadLeft(2, '0');

			return value;
		}

		public string GetPath(string path1, string path2)
		{
			var path = Path.Combine(path1, path2.TrimStart('\\'));

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}
	}
}
