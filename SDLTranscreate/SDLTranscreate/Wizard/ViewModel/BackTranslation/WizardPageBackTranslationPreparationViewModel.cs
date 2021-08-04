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
using Newtonsoft.Json;
using NLog;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Trados.Transcreate.Commands;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.FileTypeSupport.XLIFF.Writers;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;
using Trados.Transcreate.Service;
using Trados.Transcreate.Wizard.View;
using File = System.IO.File;
using IProject = Sdl.ProjectAutomation.Core.IProject;
using PathInfo = Trados.Transcreate.Common.PathInfo;
using ProjectFile = Trados.Transcreate.Model.ProjectFile;
using Task = System.Threading.Tasks.Task;

namespace Trados.Transcreate.Wizard.ViewModel.BackTranslation
{
	public class WizardPageBackTranslationPreparationViewModel : WizardPageViewModelBase
	{
		private readonly object _lockObject = new object();
		private const string ForegroundSuccess = "#017701";
		private const string ForegroundError = "#7F0505";
		private const string ForegroundProcessing = "#0096D6";

		private readonly SegmentBuilder _segmentBuilder;
		private readonly PathInfo _pathInfo;
		private readonly Controllers _controllers;
		private readonly ProjectAutomationService _projectAutomationService;
		private readonly ProjectSettingsService _projectSettingsService;
		private List<JobProcess> _jobProcesses;
		private ICommand _viewExceptionCommand;
		private ICommand _openFolderInExplorerCommand;
		private SolidColorBrush _textMessageBrush;
		private string _textMessage;
		private StringBuilder _logReport;

		public WizardPageBackTranslationPreparationViewModel(Window owner, UserControl view, TaskContext taskContext,
			SegmentBuilder segmentBuilder, PathInfo pathInfo, Controllers controllers,
			ProjectAutomationService projectAutomationService, ProjectSettingsService projectSettingsService)
			: base(owner, view, taskContext)
		{
			_segmentBuilder = segmentBuilder;
			_pathInfo = pathInfo;
			_controllers = controllers;
			_projectAutomationService = projectAutomationService;
			_projectSettingsService = projectSettingsService;

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
					Name = PluginResources.JobProcess_CreateBackTranslations
				},
				new JobProcess
				{
					Name = PluginResources.JobProcess_LoadProjects
				},
				new JobProcess
				{
					Name = PluginResources.JobProcess_Finalize
				}
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

				var taskContexts = new List<TaskContext>();
				if (success)
				{
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_CreateBackTranslations);
					if (job != null)
					{
						var result = await CreateBackTranslations(job);
						success = result.Item1;
						taskContexts = result.Item2;
					}
				}

				if (success)
				{
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_LoadProjects);
					if (job != null)
					{
						success = LoadStudioProjects(job, taskContexts);
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

				TaskContext.Completed = success;

				FinalizeJobProcesses(success);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
			}
			finally
			{
				Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new Action(delegate
				{
					IsProcessing = false;
				}));
			}

			SaveLogReport();
		}

		private async Task<bool> Preparation(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logReport.AppendLine();
				//_logReport.AppendLine("Phase: Preparation - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_Initializing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);

				//await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 30, PluginResources.JobProcess_ProcessingPleaseWait);

				//_logReport.AppendLine("Phase: Preparation - Complete " + FormatDateTime(DateTime.UtcNow));

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100, PluginResources.JobProcess_Done);
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message);
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<Tuple<bool, List<TaskContext>>> CreateBackTranslations(JobProcess jobProcess)
		{
			var taskContexts = new List<TaskContext>();
			var success = true;
			var phase = PluginResources.JobProcess_CreateBackTranslations;

			var projectFolderReferences = new List<ProjectFolderReference>();
			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.JobProcess_CreateBackTranslations;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 0,
					PluginResources.JobProcess_ReadingProjectFiles);

				var sdlxliffReader = new SdlxliffReader(_segmentBuilder, TaskContext.ExportOptions, TaskContext.AnalysisBands);
				var sdlxliffWriter = new SdlxliffWriter(_segmentBuilder, TaskContext.ImportOptions, TaskContext.AnalysisBands);
				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);

				var studioProjectInfo = TaskContext.FileBasedProject.GetProjectInfo();

				var fileDataList = GetFileDataList(TaskContext.ProjectFiles.Where(a => a.Selected).ToList(),
					TaskContext.Project, studioProjectInfo, sdlxliffReader);

				var selectedLanguages = GetSelectedLanguages().ToList();
				var totalLanguages = System.Convert.ToDouble(selectedLanguages.Count);

				var languageIndex = 0;
				foreach (var languageName in selectedLanguages)
				{
					languageIndex++;
					var unit = System.Convert.ToInt32((System.Convert.ToDouble(languageIndex) / totalLanguages) * 100);
					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, unit,
						string.Format(PluginResources.JobProcess_ProcessingLanguageFiles, languageName));

					var sourceFiles = await GetSelectedSourceFiles(languageName, fileDataList, xliffWriter);

					var backTranslationProject = TaskContext.Project.BackTranslationProjects.FirstOrDefault(a =>
						string.Compare(a.SourceLanguage.CultureInfo.Name, languageName,
							StringComparison.CurrentCultureIgnoreCase) == 0);

					var backTranslationProjectStudio = GetBackTranslationStudioProject(backTranslationProject);

					var projectReports = _projectAutomationService.GetProjectReports(backTranslationProjectStudio);

					await CloseBackTranslationProject(backTranslationProjectStudio);

					var projectFolderReference = new ProjectFolderReference
					{
						LocalProjectFolderTemp = Path.Combine(Path.GetTempPath(), GetDateTimeToString(DateTime.Now)),
						LocalProjectFolder = Path.Combine(studioProjectInfo.LocalProjectFolder, "BackProjects", languageName)
					};
					projectFolderReferences.Add(projectFolderReference);

					if (Directory.Exists(projectFolderReference.LocalProjectFolder))
					{
						await MoveBackTranslationProject(projectFolderReference.LocalProjectFolder, projectFolderReference.LocalProjectFolderTemp);
						if (backTranslationProject != null)
						{
							await PersistExistingBackTranslationFiles(backTranslationProject, languageName,
								projectFolderReference.LocalProjectFolderTemp, studioProjectInfo, sourceFiles, sdlxliffReader);
						}
					}

					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, unit, PluginResources.Progres_Label_CreatingProject);

					var iconPath = _projectAutomationService.GetBackTranslationIconPath(_pathInfo);
					var newStudioProject = await _projectAutomationService.CreateBackTranslationProject(
						TaskContext.FileBasedProject, projectFolderReference.LocalProjectFolder, languageName, iconPath,
						sourceFiles, "BT");
					projectFolderReference.Project = newStudioProject;

					_controllers.ProjectsController.RefreshProjects();
					_controllers.ProjectsController.ActivateProject(newStudioProject);

					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, unit,
						string.Format(PluginResources.Progres_Label_CreatedProject,
							newStudioProject.GetProjectInfo().Name));

					if (projectReports?.Count > 0)
					{
						var newProjectReports = _projectAutomationService.GetProjectReports(newStudioProject);
						newProjectReports.AddRange(projectReports);

						_projectAutomationService.UpdateProjectSettingsBundle(newStudioProject, newProjectReports, null,
							null);
					}

					AlignProjectFileIds(backTranslationProject, newStudioProject);

					var newStudioProjectInfo = newStudioProject.GetProjectInfo();
					var indent = "   ";

					_logReport.AppendLine();
					_logReport.AppendLine(PluginResources.Label_Project);
					_logReport.AppendLine(indent + string.Format(PluginResources.Label_Name, newStudioProjectInfo.Name));
					_logReport.AppendLine(indent + string.Format(PluginResources.Label_Location, newStudioProjectInfo.LocalProjectFolder));
					_logReport.AppendLine(indent + string.Format(PluginResources.Label_SourceLanguage, newStudioProjectInfo.SourceLanguage.CultureInfo.Name));
					_logReport.AppendLine(indent + string.Format(PluginResources.Label_TargetLanguages, newStudioProjectInfo.TargetLanguages.FirstOrDefault()?.CultureInfo.Name));
					_logReport.AppendLine(PluginResources.Label_SourceFiles);

					foreach (var sourceFile in sourceFiles)
					{
						_logReport.AppendLine(string.Format(indent + "{0}: {1}", sourceFile.Action.ToString(),
							sourceFile.FolderPathInProject + sourceFile.FileName));
					}

					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, unit, PluginResources.Progres_Label_MergingProjectFolders);
					CopyProjectFolders(projectFolderReference.LocalProjectFolderTemp, projectFolderReference.LocalProjectFolder);

					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, unit, PluginResources.Progres_Label_Translating);
					await _projectAutomationService.RunPretranslationWithoutTm(newStudioProject);

					await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, unit, PluginResources.Progres_Label_UpdatingTask);
					var taskContext = await CreateBackTranslationTaskContext(newStudioProject, backTranslationProject,
						sourceFiles, studioProjectInfo.LocalProjectFolder, projectFolderReference.LocalProjectFolderTemp,
						sdlxliffReader, sdlxliffWriter, xliffWriter);

					taskContext.Completed = true;
					taskContexts.Add(taskContext);
				}

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " -  Completed " + FormatDateTime(DateTime.UtcNow));

				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100,
					PluginResources.JobProcess_Done);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);

				jobProcess.Errors.Add(ex);
				await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message);
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));

				// Undo any existing operations to the project
				foreach (var projectFolderReference in projectFolderReferences)
				{
					await CloseBackTranslationProject(projectFolderReference.Project);
					await MoveBackTranslationProject(projectFolderReference.LocalProjectFolderTemp, projectFolderReference.LocalProjectFolder);
				}
			}
			finally
			{
				foreach (var projectFolder in projectFolderReferences)
				{
					TryDeleteFolder(projectFolder.LocalProjectFolderTemp);
				}
			}

			return await Task.FromResult(new Tuple<bool, List<TaskContext>>(success, taskContexts));
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

		private static void AlignProjectFileIds(Interfaces.IProject backTranslationProject, IProject newStudioProject)
		{
			if (backTranslationProject != null)
			{
				var newStudioProjectProjectInfo = newStudioProject.GetProjectInfo();
				backTranslationProject.Id = newStudioProjectProjectInfo.Id.ToString();

				foreach (var targetLanguage in newStudioProjectProjectInfo.TargetLanguages)
				{
					var backTranslationLanguageFiles = backTranslationProject.ProjectFiles.Where(
						a => string.Compare(a.TargetLanguage, targetLanguage.CultureInfo.Name,
							StringComparison.CurrentCultureIgnoreCase) == 0).ToList();

					if (backTranslationLanguageFiles.Any())
					{
						foreach (var targetLanguageFile in newStudioProject.GetTargetLanguageFiles(targetLanguage))
						{
							var backTranslationLanguageFile = backTranslationLanguageFiles.FirstOrDefault(a =>
								string.Compare(a.Name, targetLanguageFile.Name, StringComparison.CurrentCultureIgnoreCase) == 0 &&
								string.Compare(a.Path.Trim('\\'), targetLanguageFile.Folder.Trim('\\'), StringComparison.CurrentCultureIgnoreCase) == 0);

							if (backTranslationLanguageFile != null)
							{
								backTranslationLanguageFile.FileId = targetLanguageFile.Id.ToString();
								backTranslationLanguageFile.ProjectId = newStudioProjectProjectInfo.Id.ToString();

								foreach (var projectFileActivity in backTranslationLanguageFile.ProjectFileActivities)
								{
									projectFileActivity.ProjectFileId = backTranslationLanguageFile.FileId;
								}
							}
						}
					}
				}
			}
		}

		private FileBasedProject GetBackTranslationStudioProject(Interfaces.IProject backTranslationProject)
		{
			var backTranslationProjectStudio = _controllers.ProjectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == backTranslationProject?.Id
									 || string.Compare(Path.GetDirectoryName(a.FilePath), backTranslationProject?.Path,
										 StringComparison.CurrentCultureIgnoreCase) == 0);

			return backTranslationProjectStudio;
		}

		private bool LoadStudioProjects(JobProcess jobProcess, IEnumerable<TaskContext> taskContexts)
		{
			var success = true;

			TextMessage = PluginResources.JobProcess_LoadProjects;
			TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
			var phase = PluginResources.JobProcess_LoadProjects;

			_logReport.AppendLine();
			_logReport.AppendLine("Phase: " + phase + " - Started " + FormatDateTime(DateTime.UtcNow));

			Task.Run(() => UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 50, "Loading projects..."));

			try
			{
				foreach (var taskContext in taskContexts)
				{
					lock (_lockObject)
					{
						_projectAutomationService.ActivateProject(taskContext.FileBasedProject);

						_logReport.AppendLine();
						_logReport.AppendLine(string.Format(PluginResources.Label_LoadingProject, taskContext.FileBasedProject.FilePath));

						_controllers.TranscreateController.UpdateBackTranslationProjectData(TaskContext.Project.Id, taskContext);

						_logReport.AppendLine("Done!");
					}
				}

				_controllers.TranscreateController.RefreshProjects(true);

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: " + phase + " -  Completed " + FormatDateTime(DateTime.UtcNow));

				Task.Run(() => UpdateProgress(jobProcess, JobProcess.ProcessStatus.Completed, 100, PluginResources.JobProcess_Done));
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);

				jobProcess.Errors.Add(ex);
				Task.Run(() => UpdateProgress(jobProcess, JobProcess.ProcessStatus.Failed, jobProcess.Progress, ex.Message));
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return success;
		}

		private async Task<bool> Finalize(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logReport.AppendLine();
				//_logReport.AppendLine("Phase: Finalize - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_Finalizing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				//await UpdateProgress(jobProcess, JobProcess.ProcessStatus.Running, 30, PluginResources.JobProcess_ProcessingPleaseWait);

				Refresh();

				//_logReport.AppendLine("Phase: Finalize - Completed " + FormatDateTime(DateTime.UtcNow));
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

		private void WriteLogReportHeader()
		{
			_logReport = new StringBuilder();
			_logReport.AppendLine("Start Process: Create Back-Translation projects " + FormatDateTime(DateTime.UtcNow));
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
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_WorkingFolder, TaskContext.WorkingFolder));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_OverwriteExistingBackTranslations, TaskContext.BackTranslationOptions.OverwriteExistingBackTranslations));

			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Files);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TotalFiles, TaskContext.ProjectFiles.Count));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ImportFiles, TaskContext.ProjectFiles.Count(a => a.Selected)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()));
			_logReport.AppendLine();
		}

		private void FinalizeJobProcesses(bool success)
		{
			_logReport.AppendLine();
			_logReport.AppendLine("End Process: Create Back-Translation projects " + FormatDateTime(DateTime.UtcNow));

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

		private void CopyProjectFolders(string from, string to)
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
			{
				var fromReportsDirectory = Path.Combine(from, "Reports");
				var fromReportsViewerDirectory = Path.Combine(from, "Reports.Viewer");
				var fromWorkFlowDirectory = Path.Combine(from, "WorkFlow");

				var toReportsDirectory = Path.Combine(to, "Reports");
				var toReportsViewerDirectory = Path.Combine(to, "Reports.Viewer");
				var toWorkFlowDirectory = Path.Combine(to, "WorkFlow");

				if (Directory.Exists(fromReportsDirectory))
				{
					Copy(fromReportsDirectory, toReportsDirectory);
				}

				if (Directory.Exists(fromReportsViewerDirectory))
				{
					Copy(fromReportsViewerDirectory, toReportsViewerDirectory);
				}

				if (Directory.Exists(fromWorkFlowDirectory))
				{
					Copy(fromWorkFlowDirectory, toWorkFlowDirectory);
				}
			}), DispatcherPriority.ContextIdle);
		}

		public static void Copy(string sourceDirectory, string targetDirectory)
		{
			var diSource = new DirectoryInfo(sourceDirectory);
			var diTarget = new DirectoryInfo(targetDirectory);

			CopyAll(diSource, diTarget);
		}

		public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{
			if (!Directory.Exists(target.FullName))
			{
				Directory.CreateDirectory(target.FullName);
			}

			// Copy each file into the new directory.
			foreach (var fi in source.GetFiles())
			{
				var destFilePath = Path.Combine(target.FullName, fi.Name);
				if (!File.Exists(destFilePath))
				{
					fi.CopyTo(Path.Combine(target.FullName, fi.Name));
				}
			}

			// Copy each sub-directory using recursion.
			foreach (var diSourceSubDir in source.GetDirectories())
			{
				var nextTargetSubDir = new DirectoryInfo(diSourceSubDir.Name);
				if (!Directory.Exists(diSourceSubDir.Name))
				{
					nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
				}

				CopyAll(diSourceSubDir, nextTargetSubDir);
			}
		}

		private async Task MoveBackTranslationProject(string from, string to)
		{
			if (Directory.Exists(from))
			{
				if (Directory.Exists(to))
				{
					Directory.Delete(to, true);
				}

				Dispatcher.CurrentDispatcher.Invoke(new Action(delegate { Directory.Move(from, to); }),
					DispatcherPriority.ContextIdle);
			}

			await Task.FromResult(1);
		}

		private async Task PersistExistingBackTranslationFiles(Interfaces.IProject backTranslationProject, string targetLanguage,
			string localProjectFolderTemp, ProjectInfo studioProjectInfo,
			ICollection<SourceFile> sourceFiles, SdlxliffReader sdlxliffReader)
		{
			var projectLocalPath = Path.Combine(localProjectFolderTemp, backTranslationProject.Name + ".sdlproj");
			var projectFiles = _projectSettingsService.GetProjectFiles(projectLocalPath);

			var projectLanguageFolder = Path.Combine(localProjectFolderTemp, studioProjectInfo.SourceLanguage.CultureInfo.Name);
			var files = Directory.GetFiles(projectLanguageFolder, "*.sdlxliff", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				var fileName = Path.GetFileName(file);
				var fileFolder = Path.GetDirectoryName(file);
				var fileNameNative = fileName.Substring(0, fileName.Length - ".sdlxliff".Length);
				var folderPathInProject = fileFolder?.Substring(projectLanguageFolder.Length).Trim('\\') + '\\';

				var projectFile = projectFiles?.FirstOrDefault(a =>
					(string.Compare(a.Name + ".sdlxliff", fileName, StringComparison.CurrentCultureIgnoreCase) == 0
					 || string.Compare(a.Name, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) &&
					string.Compare(a.Path.Trim('\\') + '\\', folderPathInProject, StringComparison.CurrentCultureIgnoreCase) == 0);

				var languageFile = projectFile?.LanguageFileInfos.FirstOrDefault(a =>
					string.Compare(a.LanguageCode, targetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0);

				var data = sdlxliffReader.ReadFile(backTranslationProject.Id, languageFile?.Guid, file, targetLanguage);
				var hasEmptyTranslations = ContainsEmptyTranslations(data);
				var fileData = new FileData
				{
					Data = data,
					HasEmptyTranslations = hasEmptyTranslations
				};

				var sourceFile = new SourceFile
				{
					FilePath = file,
					FileData = fileData,
					FileName = fileName,
					FolderPathInProject = folderPathInProject
				};

				var existingSourceFile = sourceFiles.FirstOrDefault(a =>
					string.Compare(a.FileName, fileNameNative, StringComparison.CurrentCultureIgnoreCase) == 0 &&
					string.Compare(a.FolderPathInProject, folderPathInProject, StringComparison.CurrentCultureIgnoreCase) == 0);

				if (existingSourceFile == null)
				{
					sourceFile.Action = SourceFile.Actions.Persist;
					sourceFiles.Add(sourceFile);
				}
				else
				{
					if (TaskContext.BackTranslationOptions.OverwriteExistingBackTranslations)
					{
						existingSourceFile.Action = SourceFile.Actions.Overwrite;
					}
					else
					{
						sourceFile.Action = SourceFile.Actions.Persist;
						sourceFiles.Remove(existingSourceFile);
						sourceFiles.Add(sourceFile);
					}
				}
			}

			await Task.FromResult(1);
		}

		private async Task<List<SourceFile>> GetSelectedSourceFiles(string languageName, IReadOnlyCollection<FileData> fileDataList, IXliffWriter xliffWriter)
		{
			var sourceFiles = new List<SourceFile>();
			var languageFolder = GetLanguageFolder(languageName);
			var selectedTargetFiles = GetSelectedTargetFiles(languageName);

			foreach (var projectFile in selectedTargetFiles)
			{
				var fileData = fileDataList.FirstOrDefault(a => a.Data.DocInfo.DocumentId == projectFile.FileId);
				if (fileData == null)
				{
					continue;
				}

				SwitchSourceWithTargetSegments(fileData);

				var xliffFolder = GetPath(languageFolder, projectFile.Path);
				var xliffFilePath = Path.Combine(xliffFolder, RemoveSuffix(projectFile.Name, ".sdlxliff"));
				var successCreateFile = xliffWriter.WriteFile(fileData.Data, xliffFilePath, true);
				if (!successCreateFile)
				{
					throw new Exception(string.Format(
						PluginResources.Unexpected_error_while_converting_the_file, xliffFilePath));
				}

				sourceFiles.Add(new SourceFile
				{
					FilePath = xliffFilePath,
					FileData = fileData,
					FolderPathInProject = projectFile.Path.Trim('\\') + '\\',
					FileName = Path.GetFileName(xliffFilePath),
					Action = SourceFile.Actions.Add
				});
			}

			return await Task.FromResult(sourceFiles);
		}

		private async Task CloseBackTranslationProject(FileBasedProject project)
		{
			if (project != null)
			{
				_controllers.ProjectsController.Close(project);
			}

			await Task.FromResult(1);
		}

		private async Task<TaskContext> CreateBackTranslationTaskContext(FileBasedProject newStudioProject,
			Interfaces.IProject backTranslationProject,
			IReadOnlyCollection<SourceFile> sourceFiles, string localProjectFolder, string localProjectFolderTemp,
			SdlxliffReader sdlxliffReader, SdlxliffWriter sdlxliffWriter, XliffWriter xliffWriter)
		{
			var newStudioProjectInfo = newStudioProject.GetProjectInfo();

			var action = Enumerators.Action.CreateBackTranslation;
			var workFlow = Enumerators.WorkFlow.Internal;
			var setttings = GetSettings();
			setttings.ExportOptions.IncludeBackTranslations = true;
			setttings.ExportOptions.IncludeTranslations = true;
			setttings.ExportOptions.CopySourceToTarget = false;

			var taskContext = new TaskContext(action, workFlow, setttings);
			taskContext.AnalysisBands = _projectAutomationService.GetAnalysisBands(newStudioProject);

			taskContext.LocalProjectFolder = newStudioProjectInfo.LocalProjectFolder;
			taskContext.WorkflowFolder = taskContext.GetWorkflowPath();

			var workingProject = _projectAutomationService.GetProject(newStudioProject, null);
			workingProject.ProjectFiles.RemoveAll(a => a.TargetLanguage == workingProject.SourceLanguage.CultureInfo.Name);
			taskContext.Project = workingProject;
			taskContext.FileBasedProject = newStudioProject;
			taskContext.ProjectFiles = workingProject.ProjectFiles;

			foreach (var projectFile in taskContext.ProjectFiles)
			{
				var backTranslationProjectFile = backTranslationProject?.ProjectFiles.FirstOrDefault(a =>
					string.Compare(a.FileId, projectFile.FileId, StringComparison.CurrentCultureIgnoreCase) == 0);

				if (backTranslationProjectFile != null)
				{
					foreach (var fileProjectFileActivity in backTranslationProjectFile.ProjectFileActivities)
					{
						fileProjectFileActivity.ProjectFile = projectFile;
						projectFile.ProjectFileActivities.Add(fileProjectFileActivity);
					}
				}

				projectFile.Selected = true;
				var fileData = GetFileData(sourceFiles.Select(a => a.FileData), localProjectFolder, localProjectFolderTemp, projectFile);

				var success = true;
				var filePath = Path.Combine(taskContext.WorkingFolder, projectFile.Path.Trim('\\'));
				var externalFilePath = Path.Combine(filePath, projectFile.Name + ".xliff");
				var tmpInputFile = Path.GetTempFileName();

				if (fileData != null)
				{
					File.Move(tmpInputFile, tmpInputFile + ".sdlxliff");
					tmpInputFile = tmpInputFile + ".sdlxliff";

					var paragraphUnitMap = GetParagraphUnitMap(sdlxliffReader, projectFile.ProjectId, projectFile.FileId,
						projectFile.Location, projectFile.TargetLanguage);
					AlignParagraphUnitIds(fileData.Data, paragraphUnitMap);

					if (!Directory.Exists(filePath))
					{
						Directory.CreateDirectory(filePath);
					}

					xliffWriter.WriteFile(fileData.Data, externalFilePath, true);

					success = sdlxliffWriter.UpdateFile(fileData.Data, projectFile.Location, tmpInputFile, true);
				}

				if (success)
				{
					projectFile.Date = taskContext.DateTimeStamp;
					projectFile.Action = action;
					projectFile.WorkFlow = workFlow;
					projectFile.Status = Enumerators.Status.Success;
					projectFile.Report = string.Empty;
					projectFile.ExternalFilePath = externalFilePath;
					projectFile.ConfirmationStatistics = sdlxliffWriter.ConfirmationStatistics;
					projectFile.TranslationOriginStatistics = sdlxliffWriter.TranslationOriginStatistics;
				}

				var activityFile = new ProjectFileActivity
				{
					ProjectFileId = projectFile.FileId,
					ActivityId = Guid.NewGuid().ToString(),
					Action = action,
					WorkFlow = workFlow,
					Status = success ? Enumerators.Status.Success : Enumerators.Status.Error,
					Date = projectFile.Date,
					Name = Path.GetFileName(projectFile.ExternalFilePath),
					Path = Path.GetDirectoryName(projectFile.ExternalFilePath),
					Report = string.Empty,
					ProjectFile = projectFile,
					ConfirmationStatistics = projectFile.ConfirmationStatistics,
					TranslationOriginStatistics = projectFile.TranslationOriginStatistics
				};

				projectFile.ProjectFileActivities.Add(activityFile);

				File.Copy(projectFile.Location, Path.Combine(filePath, projectFile.Name));
				File.Delete(projectFile.Location);

				File.Copy(tmpInputFile, projectFile.Location, true);
				File.Delete(tmpInputFile);
			}

			return await Task.FromResult(taskContext);
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

		private Dictionary<string, List<string>> GetParagraphUnitMap(SdlxliffReader sdlxliffReader, string projectId, string fileId, string path, string targetLanguage)
		{
			var xliffData = sdlxliffReader.ReadFile(projectId, fileId, path, targetLanguage);
			return GetParagraphMap(xliffData);
		}

		private void AlignParagraphUnitIds(Xliff xliffData, Dictionary<string, List<string>> paragraphUnitMap)
		{
			var paragraphUnitIds = paragraphUnitMap.Keys.ToList();
			
			var i = 0;
			foreach (var file in xliffData.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					var paragraphUnitId = paragraphUnitIds[i++];
					transUnit.Id = paragraphUnitId;

					//var segmentPairIds = paragraphUnitMap[paragraphUnitId];
					//for (var index = 0; index < transUnit.SegmentPairs.Count; index++)
					//{
					//	var segmentPair = transUnit.SegmentPairs[index];
					//	segmentPair.Id = segmentPairIds[index];
					//}
				}
			}
		}

		private FileData GetFileData(IEnumerable<FileData> languageFileData, string localProjectFolder, string localProjectFolderTemp, ProjectFile projectFile)
		{
			foreach (var fileData in languageFileData)
			{
				if (fileData == null)
				{
					continue;
				}

				var fileDataFullPath = GetRelativePath(localProjectFolder, localProjectFolderTemp, fileData.Data.DocInfo.Source);
				var projectFilePth = Path.Combine(projectFile.TargetLanguage, projectFile.Path.Trim('\\'));
				var projectFileFullPath = Path.Combine(projectFilePth, projectFile.Name);

				if (string.Compare(fileDataFullPath, projectFileFullPath, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return fileData;
				}
			}

			return null;
		}

		private string GetRelativePath(string projectPath, string projectPathTemp, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			if (path.StartsWith(projectPath, StringComparison.CurrentCultureIgnoreCase))
			{
				return path.Replace(projectPath.Trim('\\') + '\\', string.Empty);
			}

			return path.Replace(projectPathTemp.Trim('\\') + '\\', string.Empty);
		}

		private Settings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		private static string RemoveSuffix(string name, string suffix)
		{
			return name.Substring(0, name.Length - suffix.Length);
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

		private void SwitchSourceWithTargetSegments(FileData fileData)
		{
			var sourceLanguage = fileData.Data.DocInfo.SourceLanguage;
			fileData.Data.DocInfo.SourceLanguage = fileData.Data.DocInfo.TargetLanguage;
			fileData.Data.DocInfo.TargetLanguage = sourceLanguage;

			var languagePathId = "\\" + fileData.Data.DocInfo.SourceLanguage + "\\";
			var languagePathLocation =
				fileData.Data.DocInfo.Source.LastIndexOf(languagePathId, StringComparison.CurrentCultureIgnoreCase);
			if (languagePathLocation > -1)
			{
				var prefix = fileData.Data.DocInfo.Source.Substring(0, languagePathLocation);
				var suffix = fileData.Data.DocInfo.Source.Substring(languagePathLocation + languagePathId.Length);
				fileData.Data.DocInfo.Source = prefix + "\\" + fileData.Data.DocInfo.TargetLanguage + "\\" + suffix;
			}

			foreach (var file in fileData.Data.Files)
			{
				file.SourceLanguage = fileData.Data.DocInfo.SourceLanguage;
				file.TargetLanguage = fileData.Data.DocInfo.TargetLanguage;

				foreach (var transUnit in file.Body.TransUnits)
				{
					foreach (var segmentPair in transUnit.SegmentPairs)
					{
						var source = segmentPair.Source.Clone() as Source;
						var target = segmentPair.Target.Clone() as Target;

						var sourceText = GetSegmentText(segmentPair.Source.Elements, true);
						var targetText = GetSegmentText(segmentPair.Target.Elements, true);

						if (string.IsNullOrWhiteSpace(sourceText) && string.IsNullOrWhiteSpace(targetText))
						{
							continue;
						}

						if (string.IsNullOrWhiteSpace(targetText))
						{
							target.Elements = (source.Clone() as Source).Elements;
						}

						segmentPair.Source.Elements = new List<Element>();
						segmentPair.Target.Elements = new List<Element>();

						// Remove comment tags
						// TODO: consider adding them as comments on source segment!
						foreach (var targetElement in target.Elements)
						{
							if (targetElement is ElementComment)
							{
								continue;
							}

							segmentPair.Source.Elements.Add(targetElement);
						}

						var backTranslation = segmentPair.TranslationOrigin?.GetMetaData("back-translation");
						if (backTranslation == null)
						{
							segmentPair.TranslationOrigin = null;
							segmentPair.ConfirmationLevel = ConfirmationLevel.Unspecified;
						}
						else
						{
							segmentPair.ConfirmationLevel = ConfirmationLevel.Translated;
						}
					}
				}
			}
		}

		private List<FileData> GetFileDataList(IEnumerable<ProjectFile> projectFiles, Interfaces.IProject project, ProjectInfo studioProjectInfo, SdlxliffReader sdlxliffReader)
		{
			var fileDataList = new List<FileData>();

			foreach (var projectFile in projectFiles)
			{
				var inputPath = Path.Combine(studioProjectInfo.LocalProjectFolder, projectFile.Location);
				var data = sdlxliffReader.ReadFile(project.Id, projectFile.FileId, inputPath, projectFile.TargetLanguage);
				var hasEmptyTranslations = ContainsEmptyTranslations(data);
				fileDataList.Add(new FileData
				{
					Data = data,
					HasEmptyTranslations = hasEmptyTranslations
				});
			}

			return fileDataList;
		}

		private bool ContainsEmptyTranslations(Xliff data)
		{
			foreach (var file in data.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					if ((from segmentPair in transUnit.SegmentPairs
						 let sourceText = GetSegmentText(segmentPair.Source.Elements, true)
						 let targetText = GetSegmentText(segmentPair.Target.Elements, true)
						 where !string.IsNullOrWhiteSpace(sourceText) || !string.IsNullOrWhiteSpace(targetText)
						 select targetText).Any(string.IsNullOrWhiteSpace))
					{
						return true;
					}
				}
			}

			return false;
		}

		private string GetSegmentText(IEnumerable<Element> elements, bool includeTags)
		{
			var content = string.Empty;
			foreach (var element in elements)
			{
				if (element is ElementText text)
				{
					content += text.Text;
				}

				if (!includeTags)
				{
					continue;
				}

				if (element is ElementTagPair tag)
				{
					switch (tag.Type)
					{
						case Element.TagType.TagOpen:
							content += "<bpt ";
							content += "id=\"" + tag.TagId + "\">";
							content += tag.TagContent;
							content += "</bpt>";
							break;
						case Element.TagType.TagClose:
							content += "<ept ";
							content += "id=\"" + tag.TagId + "\">";
							content += tag.TagContent;
							content += "</ept>";
							break;
					}
				}

				if (element is ElementPlaceholder placeholder)
				{
					content += "<ph ";
					content += "id=\"" + placeholder.TagId + "\">";
					content += placeholder.TagContent;
					content += "</ph>";
				}

				if (element is ElementGenericPlaceholder genericPlaceholder)
				{
					content += "<x ";
					content += "id=\"" + genericPlaceholder.TagId + "\"";
					content += " ctype=\"" + genericPlaceholder.CType + "\"";
					if (!string.IsNullOrEmpty(genericPlaceholder.TextEquivalent))
					{
						content += " equiv-text=\"" + genericPlaceholder.TextEquivalent + "\"";
					}
					content += "/>";
				}

				if (element is ElementLocked locked)
				{
					switch (locked.Type)
					{
						case Element.TagType.TagOpen:
							content += "<mrk ";
							content += "mtype=\"protected\">";
							break;
						case Element.TagType.TagClose:
							content += "</mrk>";
							break;
					}
				}

				if (element is ElementComment comment)
				{
					switch (comment.Type)
					{
						case Element.TagType.TagOpen:
							content += "<mrk ";
							content += "mtype=\"x-sdl-comment\" ";
							content += "cid=\"" + comment.Id + "\">";

							break;
						case Element.TagType.TagClose:
							content += "</mrk>";
							break;
					}
				}
			}

			return content;
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

		private string GetSelectedLanguagesString()
		{
			var selected = TaskContext.ProjectFiles.Where(a => a.Selected);

			var selectedLanguages = string.Empty;
			foreach (var name in selected.Select(a => a.TargetLanguage).Distinct())
			{
				selectedLanguages += (string.IsNullOrEmpty(selectedLanguages) ? string.Empty : ", ") + name;
			}

			return selectedLanguages;
		}

		private IEnumerable<ProjectFile> GetSelectedTargetFiles(string name)
		{
			var selected = TaskContext.ProjectFiles.Where(a => a.Selected);
			var targetFiles = selected.Where(a => Equals(a.TargetLanguage, name));
			return targetFiles;
		}

		private IEnumerable<ProjectFile> GetTargetFiles(string name)
		{
			var targetFiles = TaskContext.ProjectFiles.Where(a => Equals(a.TargetLanguage, name));
			return targetFiles;
		}

		private IEnumerable<string> GetSelectedLanguages()
		{
			var selected = TaskContext.ProjectFiles.Where(a => a.Selected);
			var selectedLanguages = selected.Select(a => a.TargetLanguage).Distinct();
			return selectedLanguages;
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

		private string GetDateTimeToString(DateTime dateTime)
		{
			var value = dateTime.Year
						+ "" + dateTime.Month.ToString().PadLeft(2, '0')
						+ "" + dateTime.Day.ToString().PadLeft(2, '0')
						+ "" + dateTime.Hour.ToString().PadLeft(2, '0')
						+ "" + dateTime.Minute.ToString().PadLeft(2, '0')
						+ "" + dateTime.Second.ToString().PadLeft(2, '0')
						+ "" + dateTime.Millisecond.ToString().PadLeft(3, '0');

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
			Dispatcher.CurrentDispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
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
