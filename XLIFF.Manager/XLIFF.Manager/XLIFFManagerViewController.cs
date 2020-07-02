using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Controls;
using Sdl.Community.XLIFF.Manager.CustomEventArgs;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.Community.XLIFF.Manager.Model.Tasks;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using AnalysisBand = Sdl.Community.XLIFF.Manager.Model.AnalysisBand;
using AutomaticTask = Sdl.Community.XLIFF.Manager.Model.Tasks.AutomaticTask;
using ConfirmationStatistics = Sdl.Community.XLIFF.Manager.Model.ConfirmationStatistics;
using ProjectFile = Sdl.Community.XLIFF.Manager.Model.ProjectFile;
using TaskFile = Sdl.Community.XLIFF.Manager.Model.Tasks.TaskFile;

namespace Sdl.Community.XLIFF.Manager
{
	[View(
		Id = "XLIFFManager_View",
		Name = "XLIFFManager_Name",
		Description = "XLIFFManager_Description",
		Icon = "Icon",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class XLIFFManagerViewController : AbstractViewController
	{
		private readonly object _lockObject = new object();
		private List<Project> _xliffProjects;
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;
		private ProjectFilesViewControl _projectFilesViewControl;
		private ProjectsNavigationViewControl _projectsNavigationViewControl;
		private ProjectFileActivityViewController _projectFileActivityViewController;
		private IStudioEventAggregator _eventAggregator;
		private ProjectsController _projectsController;
		private EditorController _editorController;
		private FilesController _filesController;
		private ImageService _imageService;
		private PathInfo _pathInfo;
		private CustomerProvider _customerProvider;
		private ProjectSettingsService _projectSettingsService;

		protected override void Initialize(IViewContext context)
		{
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_customerProvider = new CustomerProvider();
			_projectSettingsService = new ProjectSettingsService();

			ActivationChanged += OnActivationChanged;

			_eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			_eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>()?.Subscribe(OnStudioWindowCreatedNotificationEvent);

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_editorController.Opened += EditorController_Opened;

			LoadProjects();
		}

		protected override Control GetExplorerBarControl()
		{
			if (_projectsNavigationViewControl == null)
			{
				_projectsNavigationViewModel = new ProjectsNavigationViewModel(new List<Project>(), _projectsController);
				_projectsNavigationViewModel.ProjectSelectionChanged += OnProjectSelectionChanged;

				_projectsNavigationViewControl = new ProjectsNavigationViewControl(_projectsNavigationViewModel);
			}

			return _projectsNavigationViewControl;
		}

		protected override Control GetContentControl()
		{
			if (_projectFilesViewControl == null)
			{
				_projectFilesViewModel = new ProjectFilesViewModel(null);

				_projectFilesViewControl = new ProjectFilesViewControl(_projectFilesViewModel);
				_projectsNavigationViewModel.ProjectFilesViewModel = _projectFilesViewModel;
				_projectsNavigationViewModel.Projects = _xliffProjects;
			}

			return _projectFilesViewControl;
		}

		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;

		public List<Project> GetProjects()
		{
			return _xliffProjects;
		}

		public List<Project> GetSelectedProjects()
		{
			return _projectsNavigationViewModel.SelectedProjects?.Cast<Project>().ToList();
		}

		public List<ProjectFile> GetSelectedProjectFiles()
		{
			return _projectFilesViewModel.SelectedProjectFiles?.Cast<ProjectFile>().ToList();
		}

		public void UpdateProjectData(WizardContext wizardContext)
		{
			if (wizardContext == null || !wizardContext.Completed)
			{
				return;
			}

			var project = _xliffProjects.FirstOrDefault(a => a.Id == wizardContext.Project.Id);
			if (project == null)
			{
				_xliffProjects.Add(wizardContext.Project);
				project = wizardContext.Project;
			}
			else
			{
				foreach (var wcProjectFile in wizardContext.ProjectFiles)
				{
					var projectFile = project.ProjectFiles.FirstOrDefault(a => a.FileId == wcProjectFile.FileId);
					if (projectFile == null)
					{
						wcProjectFile.Project = project;
						project.ProjectFiles.Add(wcProjectFile);
					}
					else if (wcProjectFile.Selected)
					{
						projectFile.Status = wcProjectFile.Status;
						projectFile.Action = wcProjectFile.Action;
						projectFile.Date = wcProjectFile.Date;
						projectFile.XliffFilePath = wcProjectFile.XliffFilePath;
						projectFile.ConfirmationStatistics = wcProjectFile.ConfirmationStatistics;
						projectFile.TranslationOriginStatistics = wcProjectFile.TranslationOriginStatistics;
						projectFile.ProjectFileActivities = wcProjectFile.ProjectFileActivities;
					}
				}
			}

			if (_projectFilesViewModel != null)
			{
				_projectFilesViewModel.ProjectFiles = project.ProjectFiles;
			}

			if (_projectsNavigationViewModel != null)
			{
				_projectsNavigationViewModel.Projects = new List<Project>();
				_projectsNavigationViewModel.Projects = _xliffProjects;
			}			

			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == wizardContext.Project.Id);
			if (selectedProject == null)
			{
				return;
			}

			var automaticTask = CreateAutomaticTask(wizardContext, selectedProject);
			CreateHtmlReport(wizardContext, selectedProject, automaticTask);
			UpdateProjectReports(wizardContext, selectedProject, automaticTask);

			UpdateProjectSettingsBundle(project);
		}

		private void CreateHtmlReport(WizardContext wizardContext, FileBasedProject selectedProject, AutomaticTask automaticTask)
		{
			var project = _xliffProjects.FirstOrDefault(a => a.Id == wizardContext.Project.Id);
			var languageDirections = _projectSettingsService.GetLanguageDirections(selectedProject.FilePath);
			foreach (var taskReport in automaticTask.Reports)
			{
				var languageDirection = languageDirections.FirstOrDefault(a =>
					string.Compare(taskReport.LanguageDirectionGuid, a.Guid, StringComparison.CurrentCultureIgnoreCase) == 0);

				var reportName = Path.GetFileName(taskReport.PhysicalPath);
				var reportFilePath = Path.Combine(wizardContext.WorkingFolder, reportName);
				var htmlReportFilePath = CreateHtmlReportFile(reportFilePath);

				foreach (var wcProjectFile in wizardContext.ProjectFiles)
				{
					if (!wcProjectFile.Selected || string.Compare(wcProjectFile.TargetLanguage, languageDirection?.TargetLanguageCode,
						    StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						continue;
					}

					var projectFile = project?.ProjectFiles.FirstOrDefault(a => a.FileId == wcProjectFile.FileId);
					if (projectFile != null)
					{
						projectFile.Report = htmlReportFilePath;

						var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault();
						if (activityfile != null)
						{
							activityfile.Report = htmlReportFilePath;
						}
					}
				}
			}
		}

		private AutomaticTask CreateAutomaticTask(WizardContext wizardContext, FileBasedProject selectedProject)
		{
			var projectInfo = selectedProject.GetProjectInfo();
			var automaticTask = new AutomaticTask(wizardContext.Action)
			{
				CreatedAt = GetDateToString(wizardContext.DateTimeStamp),
				StartedAt = GetDateToString(wizardContext.DateTimeStamp),
				CompletedAt = GetDateToString(wizardContext.DateTimeStamp),
				CreatedBy = Environment.UserDomainName + "\\" + Environment.UserName
			};

			foreach (var wcProjectFile in wizardContext.ProjectFiles)
			{
				if (wcProjectFile.Selected)
				{
					var taskFile = new TaskFile
					{
						LanguageFileGuid = wcProjectFile.FileId
					};
					automaticTask.TaskFiles.Add(taskFile);

					var outputFile = new OutputFile
					{
						LanguageFileGuid = wcProjectFile.FileId
					};
					automaticTask.OutputFiles.Add(outputFile);
				}
			}

			var languageDirections = GetLanguageDirections(selectedProject, wizardContext);

			foreach (var languageDirection in languageDirections)
			{
				var actionType = wizardContext.Action == Enumerators.Action.Export
					? "ExportToXLIFF"
					: "ImportFromXLIFF";

				var reportName = string.Format("{0}_{1}_{2}_{3}.xml",
					actionType,
					wizardContext.DateTimeStampToString,
					languageDirection.Key.SourceLanguageCode,
					languageDirection.Key.TargetLanguageCode);

				var projectsReportsFolder = Path.Combine(projectInfo.LocalProjectFolder, "Reports");
				if (!Directory.Exists(projectsReportsFolder))
				{
					Directory.CreateDirectory(projectsReportsFolder);
				}

				var reportFile = Path.Combine(wizardContext.WorkingFolder, reportName);

				var projectReportsFilePath = Path.Combine(projectsReportsFolder, reportName);
				var relativeProjectReportsFilePath = Path.Combine("Reports", reportName);

				var settings = new XmlWriterSettings
				{
					OmitXmlDeclaration = true,
					Indent = false
				};

				var actionName = wizardContext.Action == Enumerators.Action.Export
					? "Export to XLIFF"
					: "Import from XLIFF";

				using (var writer = XmlWriter.Create(reportFile, settings))
				{
					writer.WriteStartElement("task");
					writer.WriteAttributeString("name", actionName);
					writer.WriteAttributeString("created", wizardContext.DateTimeStampToString);

					WriteReportTaskInfo(writer, wizardContext, selectedProject, languageDirection);

					foreach (var projectFile in languageDirection.Value)
					{
						WriteReportFile(writer, wizardContext, projectFile);
					}

					WriteReportTotal(writer, wizardContext, languageDirection);

					writer.WriteEndElement(); //task
				}

				// Copy to project reports folder
				File.Copy(reportFile, projectReportsFilePath, true);

				var report = new Report(wizardContext.Action)
				{
					Name = actionName,
					Description = actionName,
					LanguageDirectionGuid = languageDirection.Key.Guid,
					PhysicalPath = relativeProjectReportsFilePath
				};

				automaticTask.Reports.Add(report);
			}

			return automaticTask;
		}

		public void WriteResourceToFile(string resourceName, string fullFilePath)
		{
			if (File.Exists(fullFilePath))
			{
				return;
			}

			using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			{
				using (var file = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
				{
					resource?.CopyTo(file);
				}
			}
		}

		private string CreateHtmlReportFile(string xmlReportFullPath)
		{
			var htmlReportFilePath = xmlReportFullPath + ".html";
			var xsltName = "Report.xsl";
			var xsltFilePath = Path.Combine(_pathInfo.SettingsFolderPath, xsltName);
			var xsltResourceFilePath = "Sdl.Community.XLIFF.Manager.BatchTasks.Report.xsl";

			WriteResourceToFile(xsltResourceFilePath, xsltFilePath);

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

		private void WriteReportTotal(XmlWriter writer, WizardContext wizardContext, KeyValuePair<LanguageDirectionInfo, List<ProjectFile>> languageDirection)
		{
			writer.WriteStartElement("batchTotal");

			var totalTranslationOriginStatistics = GetTotalTranslationOriginStatistics(languageDirection.Value);
			WriteAnalysisXml(writer, totalTranslationOriginStatistics?.WordCounts, wizardContext.AnalysisBands);

			var totalConfirmationStatistics = GetTotalConfirmationStatistics(languageDirection.Value);
			WriteConfirmationXml(writer, totalConfirmationStatistics?.WordCounts);

			writer.WriteEndElement(); //batchTotal
		}

		private void WriteReportFile(XmlWriter writer, WizardContext wizardContext, ProjectFile projectFile)
		{
			writer.WriteStartElement("file");
			writer.WriteAttributeString("name", Path.Combine(projectFile.Path, projectFile.Name));
			writer.WriteAttributeString("guid", projectFile.FileId);

			WriteAnalysisXml(writer, projectFile.TranslationOriginStatistics?.WordCounts, wizardContext.AnalysisBands);
			WriteConfirmationXml(writer, projectFile.ConfirmationStatistics?.WordCounts);

			writer.WriteEndElement(); //file
		}

		private void WriteReportTaskInfo(XmlWriter writer, WizardContext wizardContext, IProject fileBasedProject, KeyValuePair<LanguageDirectionInfo, List<ProjectFile>> languageDirection)
		{
			writer.WriteStartElement("taskInfo");
			writer.WriteAttributeString("taskId", Guid.NewGuid().ToString());
			writer.WriteAttributeString("runAt", wizardContext.DateTimeStamp.ToShortDateString() + " " + wizardContext.DateTimeStamp.ToShortTimeString());

			WriteReportProject(writer, wizardContext);

			WriteReportLanguage(writer, languageDirection);

			WriteReportCustomer(writer, wizardContext);

			WriteReportTranslationProviders(writer, fileBasedProject);

			WriteReportSettings(writer, wizardContext);

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

		private static void WriteReportLanguage(XmlWriter writer, KeyValuePair<LanguageDirectionInfo, List<ProjectFile>> languageDirection)
		{
			writer.WriteStartElement("language");
			writer.WriteAttributeString("id", languageDirection.Key.TargetLanguageCode);
			writer.WriteAttributeString("name", new CultureInfo(languageDirection.Key.TargetLanguageCode).DisplayName);
			writer.WriteEndElement(); //language
		}

		private static void WriteReportCustomer(XmlWriter writer, WizardContext wizardContext)
		{
			if (!string.IsNullOrEmpty(wizardContext.Project.Customer?.Name))
			{
				writer.WriteStartElement("customer");
				writer.WriteAttributeString("name", wizardContext.Project.Customer.Name);
				writer.WriteAttributeString("email", wizardContext.Project.Customer.Email);
				writer.WriteEndElement(); //customer												  
			}
		}

		private static void WriteReportProject(XmlWriter writer, WizardContext wizardContext)
		{
			writer.WriteStartElement("project");
			writer.WriteAttributeString("name", wizardContext.Project.Name);
			writer.WriteAttributeString("number", wizardContext.Project.Id);
			if (wizardContext.Project.DueDate != DateTime.MinValue && wizardContext.Project.DueDate != DateTime.MaxValue)
			{
				writer.WriteAttributeString("dueDate",
					wizardContext.Project.DueDate.ToShortDateString() + " " + wizardContext.Project.DueDate.ToShortTimeString());
			}

			writer.WriteEndElement(); //project
		}

		private void WriteReportSettings(XmlWriter writer, WizardContext wizardContext)
		{
			writer.WriteStartElement("settings");
			writer.WriteAttributeString("xliffSupport", wizardContext.ExportOptions.XliffSupport.ToString());
			writer.WriteAttributeString("includeTranslations", wizardContext.ExportOptions.IncludeTranslations.ToString());
			writer.WriteAttributeString("copySourceToTarget", wizardContext.ExportOptions.CopySourceToTarget.ToString());
			writer.WriteAttributeString("excludeFilterItems", GetFitlerItemsString(wizardContext.ExcludeFilterItems));
			writer.WriteEndElement(); //settings
		}

		private void WriteConfirmationXml(XmlWriter writer, WordCounts wordCounts)
		{
			writer.WriteStartElement("confirmation");
			var processed = wordCounts?.Processed;
			WriteConfirmationWordCountStatistics(writer, processed, "processed");

			var ignored = wordCounts?.Ignored;
			WriteConfirmationWordCountStatistics(writer, ignored, "ignored");
			writer.WriteEndElement(); //confirmation
		}

		private void WriteAnalysisXml(XmlWriter writer, WordCounts wordCounts, IReadOnlyCollection<AnalysisBand> analysisBands)
		{
			writer.WriteStartElement("analysis");
			var processed = wordCounts?.Processed;
			WriteAnalysisWordCountStatistics(writer, processed, analysisBands, "processed");

			var ignored = wordCounts?.Ignored;
			WriteAnalysisWordCountStatistics(writer, ignored, analysisBands, "ignored");
			writer.WriteEndElement(); //analysis
		}

		private TranslationOriginStatistics GetTotalTranslationOriginStatistics(IEnumerable<ProjectFile> projectFiles)
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

					foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.Ignored)
					{
						var totalWordCount = statistics.WordCounts.Ignored.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Ignored.Add(wordCount);
						}
					}
				}
			}

			return statistics;
		}

		private ConfirmationStatistics GetTotalConfirmationStatistics(IEnumerable<ProjectFile> projectFiles)
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

					foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.Ignored)
					{
						var totalWordCount = statistics.WordCounts.Ignored.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Ignored.Add(wordCount);
						}
					}
				}
			}

			return statistics;
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

		private string GetFitlerItemsString(IEnumerable<FilterItem> filterItems)
		{
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

		private Dictionary<LanguageDirectionInfo, List<ProjectFile>> GetLanguageDirections(FileBasedProject project, WizardContext wizardContext)
		{
			var languageDirections = new Dictionary<LanguageDirectionInfo, List<ProjectFile>>();
			foreach (var language in _projectSettingsService.GetLanguageDirections(project.FilePath))
			{
				foreach (var projectFile in wizardContext.ProjectFiles)
				{
					if (!projectFile.Selected)
					{
						continue;
					}

					if (languageDirections.ContainsKey(language))
					{
						languageDirections[language].Add(projectFile);
					}
					else
					{
						languageDirections.Add(language, new List<ProjectFile> { projectFile });
					}
				}
			}

			return languageDirections;
		}

		private void UpdateProjectReports(WizardContext wizardContext, FileBasedProject project, AutomaticTask automaticTask)
		{
			if (project != null)
			{
				_projectsController.Close(project);
				_projectSettingsService.UpdateAnalysisTaskReportInfo(project, automaticTask);
				_projectsController.Add(project.FilePath);

				switch (wizardContext.Owner)
				{
					case Enumerators.Controller.Files:
						_filesController.Activate();
						break;
					case Enumerators.Controller.Projects:
						_projectsController.Activate();
						break;
				}
			}
		}

		private void UpdateProjectSettingsBundle(Project project)
		{
			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);

			if (selectedProject != null)
			{
				var settingsBundle = selectedProject.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<XliffManagerProject>();

				var projectFiles = new List<XliffManagerProjectFile>();
				foreach (var projectFile in project.ProjectFiles)
				{
					var fileActivities = new List<XliffManagerProjectFileActivity>();

					foreach (var fileActivity in projectFile.ProjectFileActivities)
					{
						var settingFileActivity = new XliffManagerProjectFileActivity
						{
							ProjectFileId = projectFile.FileId,
							ActivityId = fileActivity.ActivityId,
							Status = fileActivity.Status.ToString(),
							Action = fileActivity.Action.ToString(),
							Path = fileActivity.Path,
							Name = fileActivity.Name,
							Date = GetDateToString(fileActivity.Date),
							Report = fileActivity.Report,
							ConfirmationStatistics = fileActivity.ConfirmationStatistics,
							TranslationOriginStatistics = fileActivity.TranslationOriginStatistics
						};

						fileActivities.Add(settingFileActivity);
					}

					var settingProjectFile = new XliffManagerProjectFile
					{
						Activities = fileActivities,
						Path = projectFile.Path,
						Action = projectFile.Action.ToString(),
						Status = projectFile.Status.ToString(),
						FileId = projectFile.FileId,
						Name = projectFile.Name,
						Location = projectFile.Location,
						Date = GetDateToString(projectFile.Date),
						FileType = projectFile.FileType,
						XliffFilePath = projectFile.XliffFilePath,
						TargetLanguage = projectFile.TargetLanguage,
						Report = projectFile.Report,
						ShortMessage = projectFile.ShortMessage,
						ConfirmationStatistics = projectFile.ConfirmationStatistics,
						TranslationOriginStatistics = projectFile.TranslationOriginStatistics
					};

					projectFiles.Add(settingProjectFile);
				}

				xliffManagerProject.ProjectFiles.Value = projectFiles;

				selectedProject.UpdateSettings(xliffManagerProject.SettingsBundle);

				selectedProject.Save();
			}
		}

		private void LoadProjects()
		{
			_xliffProjects = new List<Project>();

			foreach (var project in _projectsController.GetAllProjects())
			{
				var projectInfo = project.GetProjectInfo();
				try
				{
					var settingsBundle = project.GetSettings();
					var xliffManagerProject = settingsBundle.GetSettingsGroup<XliffManagerProject>();

					var projectFiles = xliffManagerProject.ProjectFiles.Value;

					if (projectFiles?.Count > 0)
					{
						var xliffProjectFiles = new List<ProjectFile>();

						var xliffProject = new Project
						{
							Id = projectInfo.Id.ToString(),
							Name = projectInfo.Name,
							AbsoluteUri = projectInfo.Uri.AbsoluteUri,
							Customer = _customerProvider.GetProjectCustomer(project),
							Created = projectInfo.CreatedAt,
							DueDate = projectInfo.DueDate ?? DateTime.MaxValue,
							Path = projectInfo.LocalProjectFolder,
							SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo),
							TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages),
							ProjectType = GetProjectType(project),
							ProjectFiles = xliffProjectFiles
						};

						foreach (var projectFile in projectFiles)
						{
							var xliffFileActivities = new List<ProjectFileActivity>();

							var xliffProjectFile = new ProjectFile
							{
								Path = projectFile.Path,
								Action = GetAction(projectFile.Action),
								Status = GetStatus(projectFile.Status),
								Date = GetDateTime(projectFile.Date),
								Report = projectFile.Report,
								FileId = projectFile.FileId,
								FileType = projectFile.FileType,
								Location = projectFile.Location,
								Name = projectFile.Name,
								ProjectId = projectInfo.Id.ToString(),
								ProjectFileActivities = xliffFileActivities,
								XliffFilePath = projectFile.XliffFilePath,
								Project = xliffProject,
								TargetLanguage = projectFile.TargetLanguage,
								ConfirmationStatistics = projectFile.ConfirmationStatistics,
								TranslationOriginStatistics = projectFile.TranslationOriginStatistics
							};

							foreach (var fileActivity in projectFile.Activities)
							{
								var xliffFileActivity = new ProjectFileActivity
								{
									ProjectFile = xliffProjectFile,
									Path = fileActivity.Path,
									Action = GetAction(fileActivity.Action),
									Status = GetStatus(fileActivity.Status),
									ActivityId = fileActivity.ActivityId,
									Date = GetDateTime(fileActivity.Date),
									Report = fileActivity.Report,
									Name = fileActivity.Name,
									ProjectFileId = fileActivity.ProjectFileId,
									ConfirmationStatistics = fileActivity.ConfirmationStatistics,
									TranslationOriginStatistics = fileActivity.TranslationOriginStatistics
								};

								xliffFileActivities.Add(xliffFileActivity);
							}

							xliffProject.ProjectFiles.Add(xliffProjectFile);
						}

						_xliffProjects.Add(xliffProject);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(string.Format("Unable to load project: {0}", projectInfo.Name)
									+ Environment.NewLine + Environment.NewLine + ex.Message,
						PluginResources.Plugin_Name, MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
		}

		private void EditorController_Opened(object sender, DocumentEventArgs e)
		{
			if (e?.Document?.ActiveFile == null || e?.Document?.Project == null)
			{
				return;
			}

			var project = e.Document.Project;
			var projectInfo = project.GetProjectInfo();
			var projectId = projectInfo.Id.ToString();
			var documentId = e.Document.ActiveFile.Id.ToString();
			var language = e.Document.ActiveFile.Language;

			var xliffProject = _xliffProjects.FirstOrDefault(a => a.Id == projectId);
			var targetFile = xliffProject?.ProjectFiles.FirstOrDefault(a => a.FileId == documentId &&
											 a.TargetLanguage == language.CultureInfo.Name);

			if (targetFile != null && targetFile.Action == Enumerators.Action.Export)
			{
				var activityfile = targetFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Export);

				var message1 = string.Format(PluginResources.Message_FileWasExportedOn, activityfile?.DateToString);
				var message2 = string.Format(PluginResources.Message_WarningTranslationsCanBeOverwrittenDuringImport, activityfile?.DateToString);

				MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2, PluginResources.XLIFFManager_Name, MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
			}
		}

		private DateTime GetDateTime(string value)
		{
			var format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
			var successDate = DateTime.TryParseExact(value, format,
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultDate);

			var date = successDate ? resultDate : DateTime.MinValue;
			return date;
		}

		private string GetDateToString(DateTime date)
		{
			var value = string.Empty;

			if (date != DateTime.MinValue || date != DateTime.MaxValue)
			{
				return date.ToUniversalTime()
					.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
			}

			return value;
		}

		private Enumerators.Status GetStatus(string value)
		{
			var successStatus = Enum.TryParse<Enumerators.Status>(value, true, out var resultStatus);
			var status = successStatus ? resultStatus : Enumerators.Status.None;
			return status;
		}

		private Enumerators.Action GetAction(string value)
		{
			var successAction = Enum.TryParse<Enumerators.Action>(value, true, out var resultAction);
			var action = successAction ? resultAction : Enumerators.Action.None;
			return action;
		}

		private string GetProjectType(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				return internalDynamicProject.ProjectType.ToString();
			}

			return null;
		}

		private List<LanguageInfo> GetLanguageInfos(IEnumerable<Language> languages)
		{
			var targetLanguages = new List<LanguageInfo>();
			foreach (var targetLanguage in languages)
			{
				targetLanguages.Add(GetLanguageInfo(targetLanguage.CultureInfo));
			}

			return targetLanguages;
		}

		private LanguageInfo GetLanguageInfo(CultureInfo cultureInfo)
		{
			var languageInfo = new LanguageInfo
			{
				CultureInfo = cultureInfo,
				Image = _imageService.GetImage(cultureInfo.Name)
			};

			return languageInfo;
		}

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			ProjectSelectionChanged?.Invoke(this, e);
		}

		private void OnStudioWindowCreatedNotificationEvent(StudioWindowCreatedNotificationEvent e)
		{

		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			if (e.Active)
			{
				SetProjectFileActivityViewController();
				_projectFilesViewModel.Refresh();
			}
		}

		private void SetProjectFileActivityViewController()
		{
			lock (_lockObject)
			{
				if (_projectFileActivityViewController != null)
				{
					return;
				}

				try
				{
					_projectFileActivityViewController =
						SdlTradosStudio.Application.GetController<ProjectFileActivityViewController>();

					_projectFilesViewModel.ProjectFileActivityViewModel =
						new ProjectFileActivityViewModel(_projectFilesViewModel?.SelectedProjectFile?.ProjectFileActivities);

					_projectFileActivityViewController.ViewModel = _projectFilesViewModel.ProjectFileActivityViewModel;
				}
				catch
				{
					// catch all; unable to locate the controller
				}
			}
		}

		public override void Dispose()
		{
			_projectFilesViewModel?.Dispose();

			if (_editorController != null)
			{
				_editorController.Opened -= EditorController_Opened;
			}

			_projectsNavigationViewModel?.Dispose();


			base.Dispose();
		}
	}
}
