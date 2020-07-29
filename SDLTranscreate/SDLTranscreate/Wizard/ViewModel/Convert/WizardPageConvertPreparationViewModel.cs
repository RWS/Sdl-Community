using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Sdl.Community.Transcreate.Commands;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Writers;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Service;
using Sdl.Community.Transcreate.Wizard.View;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.FileBased;
using File = System.IO.File;
using ProjectFile = Sdl.Community.Transcreate.Model.ProjectFile;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.Transcreate.Wizard.ViewModel.Convert
{
	public class WizardPageConvertPreparationViewModel : WizardPageViewModelBase
	{
		private const string ForegroundSuccess = "#017701";
		private const string ForegroundError = "#7F0505";
		private const string ForegroundProcessing = "#0096D6";

		private readonly SegmentBuilder _segmentBuilder;
		private readonly PathInfo _pathInfo;
		private readonly Controllers _controllers;
		private readonly ProjectAutomationService _projectAutomationService;
		private List<JobProcess> _jobProcesses;
		private ICommand _viewExceptionCommand;
		private ICommand _openFolderInExplorerCommand;
		private SolidColorBrush _textMessageBrush;
		private string _textMessage;
		private StringBuilder _logReport;
		private FileBasedProject _newProject;

		public WizardPageConvertPreparationViewModel(Window owner, UserControl view, WizardContext wizardContext,
			SegmentBuilder segmentBuilder, PathInfo pathInfo, Controllers controllers,
			ProjectAutomationService projectAutomationService)
			: base(owner, view, wizardContext)
		{
			_segmentBuilder = segmentBuilder;
			_pathInfo = pathInfo;
			_controllers = controllers;
			_projectAutomationService = projectAutomationService;

			IsValid = true;
			InitializeJobProcessList();

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}

		public ICommand ViewExceptionCommand => _viewExceptionCommand ?? (_viewExceptionCommand = new CommandHandler(ViewException));

		public ICommand OpenFolderInExplorerCommand => _openFolderInExplorerCommand ?? (_openFolderInExplorerCommand = new CommandHandler(OpenFolderInExplorer));

		public List<JobProcess> JobProcesses
		{
			get => _jobProcesses;
			set
			{
				_jobProcesses = value;
				OnPropertyChanged(nameof(JobProcesses));
			}
		}

		public string TextMessage
		{
			get => _textMessage;
			set
			{
				_textMessage = value;
				OnPropertyChanged(nameof(TextMessage));
			}
		}

		public SolidColorBrush TextMessageBrush
		{
			get => _textMessageBrush;
			set
			{
				_textMessageBrush = value;
				OnPropertyChanged(nameof(TextMessageBrush));
			}
		}

		public override string DisplayName => PluginResources.PageName_Preparation;

		public sealed override bool IsValid { get; set; }

		private void ViewException(object obj)
		{
			var button = obj as Button;
			if (!(button?.DataContext is JobProcess jobProcess))
			{
				return;
			}

			if (jobProcess.HasErrors)
			{
				var exceptionViewer = new ExceptionViewerView();
				var model = new ExceptionViewerViewModel(jobProcess.Errors, jobProcess.Warnings);
				exceptionViewer.DataContext = model;

				exceptionViewer.ShowDialog();
			}
		}

		private void OpenFolderInExplorer(object parameter)
		{
			if (Directory.Exists(WizardContext.WorkingFolder))
			{
				Process.Start(WizardContext.WorkingFolder);
			}
		}

		private void InitializeJobProcessList()
		{
			JobProcesses = new List<JobProcess>
			{
				//new JobProcess
				//{
				//	Name = PluginResources.JobProcess_Preparation
				//},
				new JobProcess
				{
					Name = PluginResources.JobProcess_ConvertProjectFiles
				},
				new JobProcess
				{
					Name = PluginResources.JobProcess_CreateTranscreateProject
				},
				new JobProcess
				{
					Name = PluginResources.JobProcess_ImportTranslations
				}
				//new JobProcess
				//{
				//	Name = PluginResources.JobProcess_Finalize
				//}
			};
		}

		private async void StartProcessing()
		{
			try
			{
				WriteLogReportHeader();

				if (!Directory.Exists(WizardContext.WorkingFolder))
				{
					Directory.CreateDirectory(WizardContext.WorkingFolder);
				}

				var success = true;
				var job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_Preparation);
				if (job != null)
				{
					success = await Preparation(job);
				}

				if (success)
				{
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_ConvertProjectFiles);
					if (job != null)
					{
						success = await ConvertProjectFiles(job);
					}
				}

				if (success)
				{
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_CreateTranscreateProject);
					if (job != null)
					{
						success = await CreateTranscreateProject(job);
					}
				}

				if (success)
				{
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_ImportTranslations);
					if (job != null)
					{
						success = await ImportTranslations(job);
					}
				}

				if (success)
				{
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_Finalize);
					if (job != null)
					{
						success = await Finalize(job);
					}
				}

				FinalizeJobProcesses(success);
			}
			finally
			{
				Owner.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate
				{
					IsProcessing = false;
				}));
			}
		}

		private void UpdateWizardContext()
		{
			var projectFiles = WizardContext.Project.ProjectFiles;

			var newProjectInfo = _newProject.GetProjectInfo();
			WizardContext.Project = _projectAutomationService.GetProject(_newProject, null);
			AssignProjectFileXliffData(projectFiles);
			WizardContext.ProjectFiles = WizardContext.Project.ProjectFiles;
			WizardContext.AnalysisBands = _projectAutomationService.GetAnalysisBands(_newProject);
			WizardContext.LocalProjectFolder = newProjectInfo.LocalProjectFolder;
			WizardContext.TransactionFolder = WizardContext.GetDefaultTransactionPath();

			if (!Directory.Exists(WizardContext.WorkingFolder))
			{
				Directory.CreateDirectory(WizardContext.WorkingFolder);
			}
		}

		private void AssignProjectFileXliffData(IEnumerable<ProjectFile> projectfiles)
		{
			foreach (var projectFile in projectfiles)
			{
				if (projectFile.XliffData == null)
				{
					continue;
				}

				var projectFileName = projectFile.Name.Substring(0, projectFile.Name.Length - ".sdlxliff".Length) + ".xliff.sdlxliff";

				var targetFile = WizardContext.Project.ProjectFiles?.FirstOrDefault(a =>
					string.Compare(a.Name, projectFileName, StringComparison.CurrentCultureIgnoreCase) == 0
					&& string.Compare(a.Path, projectFile.Path, StringComparison.CurrentCultureIgnoreCase) == 0
					&& string.Compare(a.TargetLanguage, projectFile.TargetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0);

				if (targetFile != null)
				{
					targetFile.XliffData = projectFile.XliffData;
					targetFile.XliffFilePath = projectFile.XliffFilePath;
					targetFile.ConfirmationStatistics = projectFile.ConfirmationStatistics;
					targetFile.TranslationOriginStatistics = projectFile.TranslationOriginStatistics;
					targetFile.Selected = true;
				}
			}
		}

		private async Task<bool> Preparation(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Preparation - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_Initializing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				_logReport.AppendLine("Phase: Preparation - Complete " + FormatDateTime(DateTime.UtcNow));
				jobProcess.Status = JobProcess.ProcessStatus.Completed;

			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> ConvertProjectFiles(JobProcess jobProcess)
		{
			var success = true;
			var phase = "Convert Project Files";
			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_Initializing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				var project = WizardContext.ProjectFiles[0].Project;

				var sdlxliffReader = new SdlxliffReader(_segmentBuilder,
					WizardContext.ExportOptions, WizardContext.AnalysisBands);
				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);

				var languages = GetLanguages(project);
				foreach (var language in languages)
				{
					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, language));

					ProcessProjectFiles(language, project, sdlxliffReader, xliffWriter);
				}

				_logReport.AppendLine("Phase: " + phase + " - Complete " + FormatDateTime(DateTime.UtcNow));
				jobProcess.Status = JobProcess.ProcessStatus.Completed;

			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> CreateTranscreateProject(JobProcess jobProcess)
		{
			var success = true;
			var phase = "Create Transcreate Project";
			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_ConvertingToFormat;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				var selectedProject = _controllers.ProjectsController.GetProjects()
					.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == WizardContext.Project.Id);

				if (selectedProject == null)
				{
					throw new Exception(string.Format(PluginResources.WarningMessage_UnableToLocateProject, WizardContext.Project.Name));
				}

				var sourceLanguage = WizardContext.Project.SourceLanguage.CultureInfo.Name;

				var projectFiles = WizardContext.ProjectFiles.Where(a => IsSourceLanguage(a.TargetLanguage, sourceLanguage)).ToList();

				_newProject = _projectAutomationService.CreateTranscreateProject(selectedProject, projectFiles);
				UpdateWizardContext();

				var newProjectInfo = _newProject.GetProjectInfo();

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ProjectName, newProjectInfo.Name));
				_logReport.AppendLine(string.Format(PluginResources.Label_ProjectPath, newProjectInfo.LocalProjectFolder));
				_logReport.AppendLine(string.Format(PluginResources.Label_ProjectTemplate, selectedProject.FilePath));

				_logReport.AppendLine();
				_logReport.AppendLine(PluginResources.Label_Files);
				foreach (var projectFile in projectFiles)
				{
					_logReport.AppendLine(string.Format(PluginResources.label_XliffFile, projectFile.XliffFilePath));
				}

				if (WizardContext.ConvertOptions.CloseProjectOnComplete)
				{
					_controllers.ProjectsController.Close(selectedProject);
					_controllers.ProjectsController.RefreshProjects();
				}

				_controllers.ProjectsController.Open(_newProject);

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Completed " + FormatDateTime(DateTime.UtcNow));

				WizardContext.Completed = true;
				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> ImportTranslations(JobProcess jobProcess)
		{
			var importFiles = new List<ImportFile>();

			var success = true;
			var phase = PluginResources.JobProcess_ImportTranslations;
			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				Refresh();

				var sourceLanguage = WizardContext.Project.SourceLanguage.CultureInfo.Name;
				var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);

				var sdlxliffWriter = new SdlxliffWriter(fileTypeManager, _segmentBuilder,
					WizardContext.ImportOptions, WizardContext.AnalysisBands);

				var targetLanguages = GetLanguages(WizardContext.Project).Where(a =>
					string.Compare(a, sourceLanguage, StringComparison.CurrentCultureIgnoreCase) != 0).ToList();

				foreach (var targetLanguage in targetLanguages)
				{
					var languageFolder = GetLanguageFolder(targetLanguage);

					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, targetLanguage));

					var targetLanguageFiles = WizardContext.ProjectFiles.Where(
						a => a.XliffData != null &&
							 string.Compare(a.TargetLanguage, targetLanguage,
								 StringComparison.CurrentCultureIgnoreCase) == 0).ToList();

					foreach (var targetLanguageFile in targetLanguageFiles)
					{
						var xliffFolder = GetXliffFolder(languageFolder, targetLanguageFile);
						var xliffArchiveFile = Path.Combine(xliffFolder, targetLanguageFile.Name + ".xliff");
						var sdlXliffBackupFile = Path.Combine(xliffFolder, targetLanguageFile.Name);

						_logReport.AppendLine(string.Format(PluginResources.label_SdlXliffFile, targetLanguageFile.Location));
						if (WizardContext.ImportOptions.BackupFiles)
						{
							_logReport.AppendLine(string.Format(PluginResources.Label_BackupFile, sdlXliffBackupFile));
						}

						_logReport.AppendLine(string.Format(PluginResources.label_XliffFile, targetLanguageFile.XliffFilePath));
						_logReport.AppendLine(string.Format(PluginResources.Label_ArchiveFile, xliffArchiveFile));

						CreateBackupFile(targetLanguageFile.Location, sdlXliffBackupFile);
						CreateArchiveFile(targetLanguageFile.XliffFilePath, xliffArchiveFile);

						var sdlXliffImportFile = Path.GetTempFileName();

						var importFile = new ImportFile
						{
							SdlXliffFile = targetLanguageFile.Location,
							SdlXliffBackupFile = sdlXliffBackupFile,
							SdlXliffImportFile = sdlXliffImportFile,
							XliffFile = targetLanguageFile.XliffFilePath,
							XliffArchiveFile = xliffArchiveFile
						};
						importFiles.Add(importFile);

						success = sdlxliffWriter.UpdateFile(targetLanguageFile.XliffData, targetLanguageFile.Location, sdlXliffImportFile);

						if (success)
						{
							targetLanguageFile.Date = WizardContext.DateTimeStamp;
							targetLanguageFile.Action = Enumerators.Action.Import;
							targetLanguageFile.Status = Enumerators.Status.Success;
							targetLanguageFile.Report = string.Empty;
							targetLanguageFile.XliffFilePath = xliffArchiveFile;
							targetLanguageFile.ConfirmationStatistics = sdlxliffWriter.ConfirmationStatistics;
							targetLanguageFile.TranslationOriginStatistics = sdlxliffWriter.TranslationOriginStatistics;
						}

						var activityFile = new ProjectFileActivity
						{
							ProjectFileId = targetLanguageFile.FileId,
							ActivityId = Guid.NewGuid().ToString(),
							Action = Enumerators.Action.Import,
							Status = success ? Enumerators.Status.Success : Enumerators.Status.Error,
							Date = targetLanguageFile.Date,
							Name = Path.GetFileName(targetLanguageFile.XliffFilePath),
							Path = Path.GetDirectoryName(targetLanguageFile.XliffFilePath),
							Report = string.Empty,
							ProjectFile = targetLanguageFile,
							ConfirmationStatistics = targetLanguageFile.ConfirmationStatistics,
							TranslationOriginStatistics = targetLanguageFile.TranslationOriginStatistics
						};

						targetLanguageFile.ProjectFileActivities.Add(activityFile);

						_logReport.AppendLine(string.Format(PluginResources.Label_Success, success));
						_logReport.AppendLine();

						if (!success)
						{
							throw new Exception(string.Format(PluginResources.Message_ErrorImportingFrom, targetLanguageFile.XliffFilePath));
						}
					}
				}

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Completed " + FormatDateTime(DateTime.UtcNow));

				WizardContext.Completed = true;
				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.label_ExceptionMessage, ex.Message));
			}
			finally
			{
				if (success)
				{
					CompleteImport(importFiles);
				}
				else
				{
					UndoImport(importFiles);
				}
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> Finalize(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Finalize - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_Finalizing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();
				_logReport.AppendLine("Phase: Finalize - Completed " + FormatDateTime(DateTime.UtcNow));
				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private void CompleteImport(IEnumerable<ImportFile> importFiles)
		{
			foreach (var importFile in importFiles)
			{
				if (File.Exists(importFile.SdlXliffFile))
				{
					File.Delete(importFile.SdlXliffFile);
				}

				File.Copy(importFile.SdlXliffImportFile, importFile.SdlXliffFile, true);

				File.Delete(importFile.SdlXliffImportFile);
			}
		}

		private void UndoImport(IEnumerable<ImportFile> importFiles)
		{
			foreach (var importFile in importFiles)
			{
				if (File.Exists(importFile.SdlXliffImportFile))
				{
					File.Delete(importFile.SdlXliffImportFile);
				}

				if (File.Exists(importFile.SdlXliffBackupFile))
				{
					if (File.Exists(importFile.SdlXliffFile))
					{
						File.Delete(importFile.SdlXliffFile);
					}

					File.Copy(importFile.SdlXliffBackupFile, importFile.SdlXliffFile, true);
				}
			}
		}

		private void CreateBackupFile(string sdlXliffFile, string sdlXliffBackup)
		{
			if (File.Exists(sdlXliffBackup))
			{
				File.Delete(sdlXliffBackup);
			}

			File.Copy(sdlXliffFile, sdlXliffBackup, true);
		}

		private void CreateArchiveFile(string xliffFile, string xliffArchiveFile)
		{
			if (File.Exists(xliffArchiveFile))
			{
				File.Delete(xliffArchiveFile);
			}

			File.Copy(xliffFile, xliffArchiveFile, true);
		}

		private List<string> GetLanguages(Project project)
		{
			var languages = project.TargetLanguages.Select(a => a.CultureInfo.Name).ToList();
			languages.Add(project.SourceLanguage.CultureInfo.Name);
			return languages;
		}

		private string GetFirstTargetLanguage(Project project)
		{
			var language = GetLanguages(project).FirstOrDefault(a =>
				string.Compare(a, project.SourceLanguage.CultureInfo.Name, StringComparison.CurrentCultureIgnoreCase) != 0);
			return language;
		}

		private void ProcessProjectFiles(string language, Project project,
			SdlxliffReader sdlxliffReader, XliffWriter xliffWriter)
		{
			var languageFolder = GetLanguageFolder(language);
			var projectFiles = WizardContext.ProjectFiles.Where(a => Equals(a.TargetLanguage, language)).ToList();
			var sourceLanguage = project.SourceLanguage.CultureInfo.Name;
			List<ProjectFile> targetFiles = null;

			var isSource = IsSourceLanguage(language, sourceLanguage);
			if (isSource)
			{
				var targetLanguage = GetFirstTargetLanguage(project);
				targetFiles = WizardContext.ProjectFiles.Where(a => Equals(a.TargetLanguage, targetLanguage)).ToList();
			}

			foreach (var projectFile in projectFiles)
			{
				var xliffFolder = GetXliffFolder(languageFolder, projectFile);
				var xliffFilePath = Path.Combine(xliffFolder,
					projectFile.Name.Substring(0, projectFile.Name.Length - ".sdlxliff".Length) + ".xliff");

				var targetFile = targetFiles?.FirstOrDefault(a =>
					string.Compare(a.Name, projectFile.Name, StringComparison.CurrentCultureIgnoreCase) == 0
					&& string.Compare(a.Path, projectFile.Path, StringComparison.CurrentCultureIgnoreCase) == 0);

				if (isSource && targetFile == null)
				{
					throw new Exception(string.Format(PluginResources.ErrorMessage_ConvertingFile, projectFile.Location));
				}

				_logReport.AppendLine(string.Format(PluginResources.label_SdlXliffFile, projectFile.Location));
				_logReport.AppendLine(string.Format(PluginResources.label_XliffFile, xliffFilePath));

				projectFile.XliffData = sdlxliffReader.ReadFile(project.Id, isSource ? targetFile.Location : projectFile.Location);
				projectFile.Date = WizardContext.DateTimeStamp;
				projectFile.Action = Enumerators.Action.Export;
				projectFile.Status = Enumerators.Status.Success;
				projectFile.XliffFilePath = xliffFilePath;
				projectFile.ConfirmationStatistics = sdlxliffReader.ConfirmationStatistics;
				projectFile.TranslationOriginStatistics = sdlxliffReader.TranslationOriginStatistics;

				IncludeAlternativeStructure(projectFile.XliffData, projectFile);
				var exported = xliffWriter.WriteFile(projectFile.XliffData, xliffFilePath, !isSource);

				_logReport.AppendLine(string.Format(PluginResources.Label_Success, exported));
				_logReport.AppendLine();

				if (!exported)
				{
					throw new Exception(string.Format(PluginResources.ErrorMessage_ConvertingFile, projectFile.Location));
				}
			}
		}

		private void IncludeAlternativeStructure(Xliff xliffData, ProjectFile projectFile)
		{
			xliffData.DocInfo.TargetLanguage = null;
			xliffData.DocInfo.Source = projectFile.Location;
			xliffData.DocInfo.Comments = null;

			if (xliffData.Files?.Count > 0)
			{
				var file = xliffData.Files[0];
				file.TargetLanguage = null;
				file.Original = "Recommended";

				for (var i = 1; i <= WizardContext.ConvertOptions.MaxAlternativeTranslations; i++)
				{
					var newFile = new FileTypeSupport.XLIFF.Model.File
					{
						Original = "Alternative " + i,
						SourceLanguage = file.SourceLanguage,
						TargetLanguage = file.TargetLanguage,
						DataType = file.DataType,
						Header = file.Header,
						Body = file.Body
					};

					xliffData.Files.Add(newFile);
				}
			}
		}

		private static bool IsSourceLanguage(string language, string sourceLanguage)
		{
			return string.Compare(sourceLanguage, language, StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		private static string GetXliffFolder(string languageFolder, ProjectFile targetFile)
		{
			var xliffFolder = Path.Combine(languageFolder, targetFile.Path.TrimStart('\\'));
			if (!Directory.Exists(xliffFolder))
			{
				Directory.CreateDirectory(xliffFolder);
			}

			return xliffFolder;
		}

		private string GetLanguageFolder(string name)
		{
			var languageFolder = WizardContext.GetLanguageFolder(name);
			if (!Directory.Exists(languageFolder))
			{
				Directory.CreateDirectory(languageFolder);
			}

			return languageFolder;
		}

		private void FinalizeJobProcesses(bool success)
		{
			_logReport.AppendLine();
			_logReport.AppendLine("End Process: Convert Project " + FormatDateTime(DateTime.UtcNow));

			SaveLogReport();

			if (success)
			{
				TextMessage = PluginResources.Result_Successful;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundSuccess);
				IsComplete = true;
			}
			else
			{
				TextMessage = PluginResources.Result_Unsuccessful;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundError);
				IsComplete = false;
			}
		}

		private void SaveLogReport()
		{
			var logFileName = "log." + WizardContext.DateTimeStampToString + ".txt";
			var outputFile = Path.Combine(WizardContext.WorkingFolder, logFileName);
			using (var writer = new StreamWriter(outputFile, false, Encoding.UTF8))
			{
				writer.Write(_logReport);
				writer.Flush();
			}

			File.Copy(outputFile, Path.Combine(_pathInfo.ApplicationLogsFolderPath, logFileName));
		}

		private void WriteLogReportHeader()
		{
			_logReport = new StringBuilder();
			_logReport.AppendLine("Start Process: Convert Project " + FormatDateTime(DateTime.UtcNow));
			_logReport.AppendLine();

			var indent = "   ";
			var project = WizardContext.ProjectFiles[0].Project;
			_logReport.AppendLine(PluginResources.Label_Project);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Id, project.Id));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Name, project.Name));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Location, project.Path));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Created, project.Created.ToString(CultureInfo.InvariantCulture)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_DueDate, project.DueDate.ToString(CultureInfo.InvariantCulture)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_SourceLanguage, project.SourceLanguage.CultureInfo.DisplayName));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TargetLanguages, GetProjectTargetLanguagesString(project)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ProjectType, project.ProjectType));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Customer, project.Customer?.Name));

			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Options);

			_logReport.AppendLine(indent + string.Format(PluginResources.Label_MaxAlternativeTranslations, WizardContext.ConvertOptions.MaxAlternativeTranslations));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_UnloadOiriginalProject, WizardContext.ConvertOptions.CloseProjectOnComplete));

			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Files);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TotalFiles, WizardContext.ProjectFiles.Count));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Languages, GetProjectTargetLanguagesString(WizardContext.Project)));
			_logReport.AppendLine();
		}

		private string GetProjectTargetLanguagesString(Project project)
		{
			var targetLanguages = string.Empty;
			foreach (var languageInfo in project.TargetLanguages)
			{
				targetLanguages += (string.IsNullOrEmpty(targetLanguages) ? string.Empty : ", ") +
								   languageInfo.CultureInfo.DisplayName;
			}

			return targetLanguages;
		}
	
		private string FormatDateTime(DateTime dateTime)
		{
			var value = dateTime.Year
						+ "-" + dateTime.Month.ToString().PadLeft(2, '0')
						+ "-" + dateTime.Day.ToString().PadLeft(2, '0')
						+ "T" + dateTime.Hour.ToString().PadLeft(2, '0')
						+ ":" + dateTime.Minute.ToString().PadLeft(2, '0')
						+ ":" + dateTime.Second.ToString().PadLeft(2, '0')
						+ "." + dateTime.Millisecond.ToString().PadLeft(2, '0');

			return value;
		}

		private void Refresh()
		{
			Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
		}

		private void OnLoadPage(object sender, EventArgs e)
		{
			IsProcessing = true;
			Refresh();
			StartProcessing();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
