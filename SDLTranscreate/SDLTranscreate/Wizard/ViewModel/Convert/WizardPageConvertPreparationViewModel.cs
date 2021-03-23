using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using NLog;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.FileBased;
using Trados.Transcreate.Commands;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.FileTypeSupport.XLIFF.Writers;
using Trados.Transcreate.Model;
using Trados.Transcreate.Service;
using Trados.Transcreate.Wizard.View;
using Button = System.Windows.Controls.Button;
using File = System.IO.File;
using ProjectFile = Trados.Transcreate.Model.ProjectFile;
using Task = System.Threading.Tasks.Task;

namespace Trados.Transcreate.Wizard.ViewModel.Convert
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

		public WizardPageConvertPreparationViewModel(Window owner, object view, TaskContext taskContext,
			SegmentBuilder segmentBuilder, PathInfo pathInfo, Controllers controllers,
			ProjectAutomationService projectAutomationService)
			: base(owner, view, taskContext)
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
			if (Directory.Exists(TaskContext.WorkingFolder))
			{
				Process.Start(TaskContext.WorkingFolder);
			}
		}

		private void InitializeJobProcessList()
		{
			JobProcesses = new List<JobProcess>
			{
				new JobProcess
				{
					Name = PluginResources.JobProcess_Preparation
				},
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

				if (!Directory.Exists(TaskContext.WorkingFolder))
				{
					Directory.CreateDirectory(TaskContext.WorkingFolder);
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

					_projectAutomationService.ActivateProject(_newProject);
					_controllers.ProjectsController.RefreshProjects();
				}

				FinalizeJobProcesses(success);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
			}
			finally
			{
				Owner.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate
				{
					IsProcessing = false;
				}));
			}
		}

		private async Task<bool> Preparation(JobProcess jobProcess)
		{
			var success = true;
			var phase = PluginResources.JobProcess_Preparation;
			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_Initializing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 0, PluginResources.JobProcess_ProcessingPleaseWait);

				_logReport.AppendLine("Phase: Preparation - Complete " + FormatDateTime(DateTime.UtcNow));

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100, PluginResources.JobProcess_Done);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message);
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> ConvertProjectFiles(JobProcess jobProcess)
		{
			var success = true;
			var phase = PluginResources.JobProcess_ConvertProjectFiles;
			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_ConvertingToFormat;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 0, PluginResources.JobProcess_ConvertingProjectFiles);

				var project = TaskContext.ProjectFiles[0].Project;

				var selectedProject = _controllers.ProjectsController.GetProjects()
					.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == TaskContext.Project.Id);
				await _projectAutomationService.RunPretranslationWithoutTm(selectedProject);

				_projectAutomationService.RemoveLastReportOfType("Translate");

				var sdlxliffReader = new SdlxliffReader(_segmentBuilder, TaskContext.ExportOptions, TaskContext.AnalysisBands);
				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);

				var sourceLanguage = TaskContext.Project.SourceLanguage.CultureInfo.Name;
				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_Language, sourceLanguage));

				var total = GetTargetLangauges(TaskContext.Project).Count;
				var unit = System.Convert.ToInt32(Math.Truncate(System.Convert.ToDouble(100 / ((total * 2) + 1))));
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, unit, string.Format(PluginResources.JobProcess_ProcessingLanguageFiles, project.SourceLanguage.CultureInfo.DisplayName));

				var sourceProjectFiles = await ProcessProjectFiles(sourceLanguage, project, sdlxliffReader);

				var targetProjectFiles = new List<ProjectFile>();
				var targetLangauges = GetTargetLangauges(project);
				foreach (var targetLanguage in targetLangauges)
				{
					var cultureInfo = new Language(new CultureInfo(targetLanguage));
					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, jobProcess.Progress + unit, string.Format(PluginResources.JobProcess_ProcessingLanguageFiles, cultureInfo.DisplayName));

					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, targetLanguage));

					var targetFiles = await ProcessProjectFiles(targetLanguage, project, sdlxliffReader);
					targetProjectFiles.AddRange(targetFiles);
				}

				ClearTargetTransUnits(sourceProjectFiles);
				WriteXliffFiles(xliffWriter, sourceProjectFiles);

				foreach (var projectFile in targetProjectFiles)
				{
					var cultureInfo = new Language(new CultureInfo(projectFile.TargetLanguage));
					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, jobProcess.Progress + unit, string.Format(PluginResources.JobProcess_ConvertingLanguageFiles, cultureInfo.DisplayName));

					var sourceProjectFile = sourceProjectFiles?.FirstOrDefault(a =>
						string.Compare(a.Name, projectFile.Name, StringComparison.CurrentCultureIgnoreCase) == 0
						&& string.Compare(a.Path, projectFile.Path, StringComparison.CurrentCultureIgnoreCase) == 0);

					if (sourceProjectFile == null)
					{
						continue;
					}

					projectFile.Action = Enumerators.Action.Convert;

					// add alternative files to the target
					for (var i = 1; i < sourceProjectFile.XliffData.Files.Count; i++)
					{
						if (sourceProjectFile.XliffData.Files[i].Clone() is FileTypeSupport.XLIFF.Model.File file)
						{
							file.TargetLanguage = projectFile.TargetLanguage;
							projectFile.XliffData.Files.Add(file);
						}
					}

					WriteXliffFile(xliffWriter, projectFile);
				}

				_logReport.AppendLine("Phase: " + phase + " - Complete " + FormatDateTime(DateTime.UtcNow));

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100, PluginResources.JobProcess_Done);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message);
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> CreateTranscreateProject(JobProcess jobProcess)
		{
			var success = true;
			var phase = PluginResources.JobProcess_CreateTranscreateProject;
			var newProjectLocalFolder = string.Empty;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_CreatingTranscreateProject;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 0, PluginResources.JobProcess_ProcessingPleaseWait);

				var selectedProject = _controllers.ProjectsController.GetProjects()
					.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == TaskContext.Project.Id);

				if (selectedProject == null)
				{
					throw new Exception(string.Format(PluginResources.WarningMessage_UnableToLocateProject, TaskContext.Project.Name));
				}

				newProjectLocalFolder = selectedProject.GetProjectInfo().LocalProjectFolder + "-T";
				if (Directory.Exists(newProjectLocalFolder))
				{
					throw new Exception(PluginResources.Warning_Message_ProjectFolderAlreadyExists + Environment.NewLine + Environment.NewLine + newProjectLocalFolder);
				}

				var sourceLanguage = TaskContext.Project.SourceLanguage.CultureInfo.Name;
				var projectFiles = TaskContext.ProjectFiles.Where(a => IsSourceLanguage(a.TargetLanguage, sourceLanguage)).ToList();

				if (projectFiles.Count == 0)
				{
					throw new Exception(PluginResources.Warning_Message_NoSourceFilesFound);
				}

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 30, PluginResources.JobProcess_ProcessingPleaseWait);

				var iconPath = _projectAutomationService.GetTranscreateIconPath(_pathInfo);

				_newProject = _projectAutomationService.CreateTranscreateProject(selectedProject, iconPath, projectFiles, "T");
				_controllers.ProjectsController.RefreshProjects();

				UpdateWizardContext(_newProject);

				var newProjectInfo = _newProject.GetProjectInfo();

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ProjectName, newProjectInfo.Name));
				_logReport.AppendLine(string.Format(PluginResources.Label_ProjectPath, newProjectInfo.LocalProjectFolder));
				_logReport.AppendLine(string.Format(PluginResources.Label_ProjectTemplate, selectedProject.FilePath));

				_logReport.AppendLine();
				_logReport.AppendLine(PluginResources.Label_Files);

				foreach (var projectFile in projectFiles)
				{
					_logReport.AppendLine(string.Format(PluginResources.Label_XliffFile, projectFile.ExternalFilePath));
				}

				if (TaskContext.ConvertOptions.CloseProjectOnComplete)
				{
					_controllers.ProjectsController.Close(selectedProject);
				}

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Completed " + FormatDateTime(DateTime.UtcNow));

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100, PluginResources.JobProcess_Done);
			}
			catch (Exception ex)
			{
				success = false;

				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message);

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));

				if (_newProject != null)
				{
					await CloseBackTranslationProject(_newProject);
					// cleanup folders
					TryDeleteFolder(newProjectLocalFolder);
				}
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
				TextMessage = PluginResources.WizardMessage_ImportingTranslations;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 0, PluginResources.JobProcess_ProcessingPleaseWait);

				var totalFilesCount = TaskContext.ProjectFiles.Count(a => a.XliffData != null);
				var unit = System.Convert.ToInt32(Math.Truncate(System.Convert.ToDouble(100 / totalFilesCount)));

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				var sdlxliffWriter = new SdlxliffWriter(_segmentBuilder,
					TaskContext.ImportOptions, TaskContext.AnalysisBands);

				var sdlxliffReader = new SdlxliffReader(_segmentBuilder, TaskContext.ExportOptions,
					TaskContext.AnalysisBands);

				foreach (var targetLanguage in GetTargetLangauges(TaskContext.Project))
				{
					var languageFolder = GetLanguageFolder(targetLanguage);

					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, targetLanguage));

					var targetLanguageFiles = TaskContext.ProjectFiles.Where(
						a => a.XliffData != null &&
							 string.Compare(a.TargetLanguage, targetLanguage,
								 StringComparison.CurrentCultureIgnoreCase) == 0).ToList();

					foreach (var targetLanguageFile in targetLanguageFiles)
					{
						await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, jobProcess.Progress + unit, string.Format(PluginResources.JobProcess_ImportingTranslations, targetLanguageFile.Name));

						var folder = GetXliffFolder(languageFolder, targetLanguageFile);
						var archiveFile = Path.Combine(folder, targetLanguageFile.Name + ".xliff");
						var sdlXliffBackupFile = Path.Combine(folder, targetLanguageFile.Name);

						_logReport.AppendLine(string.Format(PluginResources.Label_SdlXliffFile, targetLanguageFile.Location));
						if (TaskContext.ImportOptions.BackupFiles)
						{
							_logReport.AppendLine(string.Format(PluginResources.Label_BackupFile, sdlXliffBackupFile));
						}

						_logReport.AppendLine(string.Format(PluginResources.Label_XliffFile, targetLanguageFile.ExternalFilePath));
						_logReport.AppendLine(string.Format(PluginResources.Label_ArchiveFile, archiveFile));

						CreateBackupFile(targetLanguageFile.Location, sdlXliffBackupFile);
						CreateArchiveFile(targetLanguageFile.ExternalFilePath, archiveFile);

						var sdlXliffImportFile = Path.GetTempFileName();
						File.Move(sdlXliffImportFile, sdlXliffImportFile + ".sdlxliff");
						sdlXliffImportFile = sdlXliffImportFile + ".sdlxliff";

						var importFile = new ImportFile
						{
							SdlXliffFile = targetLanguageFile.Location,
							SdlXliffBackupFile = sdlXliffBackupFile,
							SdlXliffImportFile = sdlXliffImportFile,
							ImportFilePath = targetLanguageFile.ExternalFilePath,
							ArchiveFilePath = archiveFile
						};
						importFiles.Add(importFile);

						var paragraphMap = GetParagraphMap(sdlxliffReader, targetLanguageFile.FileId, targetLanguageFile.Location, targetLanguageFile.TargetLanguage);
						AlignParagraphIds(targetLanguageFile.XliffData, paragraphMap.Keys.ToList());

						success = sdlxliffWriter.UpdateFile(targetLanguageFile.XliffData, targetLanguageFile.Location, sdlXliffImportFile);

						if (success)
						{
							targetLanguageFile.Date = TaskContext.DateTimeStamp;
							targetLanguageFile.Action = Enumerators.Action.Convert;
							targetLanguageFile.WorkFlow = Enumerators.WorkFlow.Internal;
							targetLanguageFile.Status = Enumerators.Status.Success;
							targetLanguageFile.Report = string.Empty;
							targetLanguageFile.ExternalFilePath = archiveFile;
							targetLanguageFile.ConfirmationStatistics = sdlxliffWriter.ConfirmationStatistics;
							targetLanguageFile.TranslationOriginStatistics = sdlxliffWriter.TranslationOriginStatistics;
						}

						var activityFile = new ProjectFileActivity
						{
							ProjectFileId = targetLanguageFile.FileId,
							ActivityId = Guid.NewGuid().ToString(),
							Action = Enumerators.Action.Convert,
							WorkFlow = Enumerators.WorkFlow.Internal,
							Status = success ? Enumerators.Status.Success : Enumerators.Status.Error,
							Date = targetLanguageFile.Date,
							Name = Path.GetFileName(targetLanguageFile.ExternalFilePath),
							Path = Path.GetDirectoryName(targetLanguageFile.ExternalFilePath),
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
							throw new Exception(string.Format(PluginResources.Message_ErrorImportingFrom, targetLanguageFile.ExternalFilePath));
						}
					}
				}

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Completed " + FormatDateTime(DateTime.UtcNow));

				TaskContext.Completed = true;

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100, PluginResources.JobProcess_Done);
			}
			catch (Exception ex)
			{
				success = false;

				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message);

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
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
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 30, PluginResources.JobProcess_ProcessingPleaseWait);

				_logReport.AppendLine("Phase: Finalize - Completed " + FormatDateTime(DateTime.UtcNow));

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100, PluginResources.JobProcess_Done);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message);
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task CloseBackTranslationProject(FileBasedProject project)
		{
			if (project != null)
			{
				_controllers.ProjectsController.Close(project);
			}

			await Task.FromResult(1);
		}

		private static void TryDeleteFolder(string folder)
		{
			try
			{
				if (Directory.Exists(folder))
				{
					Directory.Delete(folder, true);
				}
			}
			catch
			{
				// catch all; ignore
			}
		}

		private Dictionary<string, List<string>> GetParagraphMap(SdlxliffReader sdlxliffReader, string fileId, string path, string targetLanguage)
		{
			var xliffData = sdlxliffReader.ReadFile(TaskContext.Project.Id, fileId, path, targetLanguage);
			return GetParagraphMap(xliffData);
		}

		private Dictionary<string, List<string>> GetParagraphMap(Xliff xliffData)
		{
			var paragraphMap = new Dictionary<string, List<string>>();

			foreach (var file in xliffData.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					if (paragraphMap.ContainsKey(transUnit.Id))
					{
						continue;
					}

					var segmentIds = new List<string>();
					foreach (var segmentPair in transUnit.SegmentPairs)
					{
						segmentIds.Add(segmentPair.Id);
					}

					paragraphMap.Add(transUnit.Id, segmentIds);
				}
			}

			return paragraphMap;
		}



		private void WriteXliffFile(XliffWriter xliffWriter, ProjectFile projectFile)
		{
			var exported = xliffWriter.WriteFile(projectFile.XliffData, projectFile.ExternalFilePath, true);
			if (!exported)
			{
				throw new Exception(string.Format(PluginResources.ErrorMessage_ConvertingFile, projectFile.Location));
			}
		}

		private void WriteXliffFiles(XliffWriter xliffWriter, IEnumerable<ProjectFile> projectFiles)
		{
			foreach (var projectFile in projectFiles)
			{
				WriteXliffFile(xliffWriter, projectFile);
			}
		}

		private void AlignParagraphIds(Xliff xliffData, IReadOnlyList<string> paragraphIds)
		{
			var i = 0;
			foreach (var file in xliffData.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					transUnit.Id = paragraphIds[i++];
				}
			}
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

		private static void ClearTargetTransUnits(List<ProjectFile> sourceProjectFiles)
		{
			foreach (var projectFile in sourceProjectFiles)
			{
				foreach (var file in projectFile.XliffData.Files)
				{
					foreach (var transUnit in file.Body.TransUnits)
					{
						foreach (var segmentPair in transUnit.SegmentPairs)
						{
							segmentPair.Target = new Target();
						}
					}
				}
			}
		}

		private void UpdateWizardContext(FileBasedProject project)
		{
			var projectFiles = TaskContext.Project.ProjectFiles;

			var newProjectInfo = project.GetProjectInfo();
			TaskContext.Project = _projectAutomationService.GetProject(project, null, projectFiles);
			TaskContext.ProjectFiles = TaskContext.Project.ProjectFiles;
			TaskContext.AnalysisBands = _projectAutomationService.GetAnalysisBands(project);
			TaskContext.LocalProjectFolder = newProjectInfo.LocalProjectFolder;
			TaskContext.WorkflowFolder = TaskContext.GetWorkflowPath();

			if (!Directory.Exists(TaskContext.WorkingFolder))
			{
				Directory.CreateDirectory(TaskContext.WorkingFolder);
			}
		}

		private IEnumerable<string> GetAllLanguages(Interfaces.IProject project)
		{
			var languages = project.TargetLanguages.Select(a => a.CultureInfo.Name).ToList();
			languages.Add(project.SourceLanguage.CultureInfo.Name);
			return languages;
		}

		private string GetFirstTargetLanguage(Interfaces.IProject project)
		{
			var language = GetAllLanguages(project).FirstOrDefault(a =>
				string.Compare(a, project.SourceLanguage.CultureInfo.Name, StringComparison.CurrentCultureIgnoreCase) != 0);
			return language;
		}

		private List<string> GetTargetLangauges(Interfaces.IProject project)
		{
			var languages = GetAllLanguages(project).Where(a =>
				string.Compare(a, project.SourceLanguage.CultureInfo.Name, StringComparison.CurrentCultureIgnoreCase) != 0).ToList();
			return languages;
		}

		private async Task<List<ProjectFile>> ProcessProjectFiles(string language, Interfaces.IProject project, SdlxliffReader sdlxliffReader)
		{
			var languageFolder = GetLanguageFolder(language);
			var projectFiles = TaskContext.ProjectFiles.Where(a => Equals(a.TargetLanguage, language)).ToList();
			var sourceLanguage = project.SourceLanguage.CultureInfo.Name;
			List<ProjectFile> targetFiles = null;

			var isSource = IsSourceLanguage(language, sourceLanguage);
			if (isSource)
			{
				var targetLanguage = GetFirstTargetLanguage(project);
				targetFiles = TaskContext.ProjectFiles.Where(a => Equals(a.TargetLanguage, targetLanguage)).ToList();
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

				_logReport.AppendLine(string.Format(PluginResources.Label_SdlXliffFile, projectFile.Location));
				_logReport.AppendLine(string.Format(PluginResources.Label_XliffFile, xliffFilePath));

				projectFile.XliffData = sdlxliffReader.ReadFile(project.Id, projectFile.FileId,
					isSource ? targetFile.Location : projectFile.Location,
					isSource ? targetFile.TargetLanguage : projectFile.TargetLanguage);
				projectFile.Date = TaskContext.DateTimeStamp;
				projectFile.Action = Enumerators.Action.Export;
				projectFile.WorkFlow = Enumerators.WorkFlow.Internal;
				projectFile.Status = Enumerators.Status.Success;
				projectFile.ExternalFilePath = xliffFilePath;
				projectFile.ConfirmationStatistics = sdlxliffReader.ConfirmationStatistics;
				projectFile.TranslationOriginStatistics = sdlxliffReader.TranslationOriginStatistics;

				if (isSource)
				{
					AddAlternativeStructure(projectFile.XliffData, projectFile);
				}
			}

			return await Task.FromResult(projectFiles);
		}

		private void AddAlternativeStructure(Xliff xliffData, ProjectFile projectFile)
		{
			xliffData.DocInfo.TargetLanguage = null;
			xliffData.DocInfo.Source = projectFile.Location;
			xliffData.DocInfo.Comments = null;

			if (xliffData.Files?.Count != 1)
			{
				throw new Exception("Unexpected inner file count");
			}

			if (xliffData.Files[0] is FileTypeSupport.XLIFF.Model.File file)
			{
				file.TargetLanguage = null;
				file.Original = "Recommended";
				xliffData.Files[0] = file.Clone() as FileTypeSupport.XLIFF.Model.File;

				var nextSegmentId = GetNextSegmentId(file);
				for (var i = 1; i <= TaskContext.ConvertOptions.MaxAlternativeTranslations; i++)
				{
					var newFile = new FileTypeSupport.XLIFF.Model.File
					{
						Original = "Alternative " + i,
						SourceLanguage = file.SourceLanguage,
						TargetLanguage = file.TargetLanguage,
						DataType = file.DataType,
						Header = file.Header
					};

					foreach (var transUnit in file.Body.TransUnits)
					{
						var tu = new TransUnit
						{
							Id = Guid.NewGuid().ToString(),
						};

						foreach (var segmentPair in transUnit.SegmentPairs)
						{
							if (segmentPair.Clone() is SegmentPair sp)
							{
								sp.Id = nextSegmentId.ToString();
								sp.Target = new Target();
								sp.ConfirmationLevel = ConfirmationLevel.Unspecified;
								sp.IsLocked = false;
								sp.TranslationOrigin = null;
								tu.SegmentPairs.Add(sp);

								nextSegmentId++;
							}
						}

						foreach (var context in transUnit.Contexts)
						{
							tu.Contexts.Add(context.Clone() as Context);
						}

						newFile.Body.TransUnits.Add(tu);
					}

					xliffData.Files.Add(newFile);
				}
			}
		}

		private static long GetNextSegmentId(FileTypeSupport.XLIFF.Model.File file)
		{
			long nextSegmentId = 0;
			var lastSegmentPair = file.Body.TransUnits.LastOrDefault()?.SegmentPairs.LastOrDefault();
			if (lastSegmentPair != null)
			{
				long id;
				if (long.TryParse(lastSegmentPair.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
				{
					nextSegmentId = id + 1;
				}
				else
				{
					// not an integer - fallback: increment by the number of segments in the paragraph
					nextSegmentId = file.Body.TransUnits.Sum(transUnit => transUnit.SegmentPairs.LongCount());
				}
			}

			return nextSegmentId;
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
			var languageFolder = TaskContext.GetLanguageFolder(name);
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
			var logFileName = "log." + TaskContext.DateTimeStampToString + ".txt";
			var outputFile = Path.Combine(TaskContext.WorkingFolder, logFileName);
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
			var project = TaskContext.ProjectFiles[0].Project;
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

			_logReport.AppendLine(indent + string.Format(PluginResources.Label_MaxAlternativeTranslations, TaskContext.ConvertOptions.MaxAlternativeTranslations));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_UnloadOiriginalProject, TaskContext.ConvertOptions.CloseProjectOnComplete));

			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Files);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TotalFiles, TaskContext.ProjectFiles.Count));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Languages, GetProjectTargetLanguagesString(TaskContext.Project)));
			_logReport.AppendLine();
		}

		private string GetProjectTargetLanguagesString(Interfaces.IProject project)
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

		private async Task UpdateProgress(JobProcess jobProcess, JobProcess.ProcessStatus status, int progress, string description)
		{
			if (progress > 100)
			{
				progress = 100;
			}

			jobProcess.Status = status;
			jobProcess.Progress = jobProcess.Progress <= progress ? progress : 100;
			jobProcess.Description = description;

			Refresh();
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
