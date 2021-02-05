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
using Sdl.Community.Transcreate.Commands;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Writers;
using Sdl.Community.Transcreate.Interfaces;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.Community.Transcreate.Service;
using Sdl.Community.Transcreate.Wizard.View;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using File = System.IO.File;
using IProject = Sdl.ProjectAutomation.Core.IProject;
using ProjectFile = Sdl.Community.Transcreate.Model.ProjectFile;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.Transcreate.Wizard.ViewModel.BackTranslation
{
	public class WizardPageBackTranslationPreparationViewModel : WizardPageViewModelBase
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

		public WizardPageBackTranslationPreparationViewModel(Window owner, UserControl view, TaskContext taskContext,
			SegmentBuilder segmentBuilder, PathInfo pathInfo, Controllers controllers, ProjectAutomationService projectAutomationService)
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
				//new JobProcess
				//{
				//	Name = PluginResources.JobProcess_Preparation
				//},
				new JobProcess
				{
					Name = PluginResources.JobProcess_CreateBackTranslations
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

				if (success)
				{
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_CreateBackTranslations);
					if (job != null)
					{
						var result = await CreateBackTranslations(job);
						success = result.Item1;
						if (success)
						{
							UpdateStudio(result.Item2);
						}
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

			SaveLogReport();
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
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<Tuple<bool, List<TaskContext>>> CreateBackTranslations(JobProcess jobProcess)
		{
			var taskContexts = new List<TaskContext>();
			var success = true;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Export - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_ConvertingToFormat;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				var sdlxliffReader = new SdlxliffReader(_segmentBuilder, TaskContext.ExportOptions, TaskContext.AnalysisBands);
				var sdlxliffWriter = new SdlxliffWriter(_segmentBuilder, TaskContext.ImportOptions, TaskContext.AnalysisBands);
				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);

				var studioProjectInfo = TaskContext.FileBasedProject.GetProjectInfo();
				var fileDataList = GetFileDataList(TaskContext.ProjectFiles.Where(a => a.Selected).ToList(), TaskContext.Project, studioProjectInfo, sdlxliffReader);

				var selectedLanguages = GetSelectedLanguages();

				foreach (var languageName in selectedLanguages)
				{
					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, languageName));

					var sourceFiles = GetSelectedSourceFiles(languageName, fileDataList, xliffWriter);

					var backTranslationProjectStudio = CloseBackTranslationProject(languageName);

					var localProjectFolderTemp = Path.Combine(Path.GetTempPath(), GetDateTimeToString(DateTime.Now));
					var localProjectFolder = Path.Combine(studioProjectInfo.LocalProjectFolder, "BackProjects", languageName, string.Empty);
					if (Directory.Exists(localProjectFolder))
					{
						MoveBackTranslationProject(localProjectFolder, localProjectFolderTemp);
						PersistExistingBackTranslationFiles(backTranslationProjectStudio, languageName, localProjectFolderTemp, studioProjectInfo, sourceFiles, sdlxliffReader);
					}

					var iconPath = GetBackTranslationIconPath();
					var newStudioProject = _projectAutomationService.CreateBackTranslationProject(
						TaskContext.FileBasedProject, localProjectFolder, languageName, iconPath,
						sourceFiles, "BT");

					_controllers.ProjectsController.RefreshProjects();

					// Copy folders here
					// Copy/merge folders to new project
					var previousReportsDirectory = Path.Combine(localProjectFolderTemp, "Reports");
					var previousReportsViewerDirectory = Path.Combine(localProjectFolderTemp, "Reports.Viewer");
					var previousWorkFlowDirectory = Path.Combine(localProjectFolderTemp, "WorkFlow");

					var newReportsDirectory = Path.Combine(localProjectFolder, "Reports");
					var newReportsViewerDirectory = Path.Combine(localProjectFolder, "Reports.Viewer");
					var newWorkFlowDirectory = Path.Combine(localProjectFolder, "WorkFlow");

					if (Directory.Exists(previousReportsDirectory))
					{
						Copy(previousReportsDirectory, newReportsDirectory);
					}

					if (Directory.Exists(previousReportsViewerDirectory))
					{
						Copy(previousReportsViewerDirectory, newReportsViewerDirectory);
					}

					if (Directory.Exists(previousWorkFlowDirectory))
					{
						Copy(previousWorkFlowDirectory, newWorkFlowDirectory);
					}

					_projectAutomationService.RunPretranslationWithoutTm(newStudioProject);

					var taskContext = CreateBackTranslationTaskContext(newStudioProject,
						sourceFiles, studioProjectInfo.LocalProjectFolder, localProjectFolderTemp,
						 sdlxliffReader, sdlxliffWriter, xliffWriter);

					taskContext.Completed = true;
					taskContexts.Add(taskContext);

					if (Directory.Exists(localProjectFolderTemp))
					{
						Directory.Delete(localProjectFolderTemp, true);
					}
				}

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Export - Completed " + FormatDateTime(DateTime.UtcNow));

				TaskContext.Completed = true;
				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(new Tuple<bool, List<TaskContext>>(success, taskContexts));
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

		private static void MoveBackTranslationProject(string from, string to)
		{
			try
			{
				if (Directory.Exists(to))
				{
					Directory.Delete(to, true);
				}

				Directory.Move(from, to);
			}
			catch
			{
				//ignore; catch all
			}
		}

		private void PersistExistingBackTranslationFiles(IProject backTranslationProjectStudio, string targetLanguage,
			string localProjectFolderTemp, ProjectInfo studioProjectInfo,
			ICollection<SourceFile> sourceFiles, SdlxliffReader sdlxliffReader)
		{
			var projectLanguageFolder = Path.Combine(localProjectFolderTemp, studioProjectInfo.SourceLanguage.CultureInfo.Name);
			var files = Directory.GetFiles(projectLanguageFolder, "*.sdlxliff", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				var fileName = Path.GetFileName(file);
				var fileFolder = Path.GetDirectoryName(file);
				var fileNameNative = fileName.Substring(0, fileName.Length - ".sdlxliff".Length);
				var folderPathInProject = fileFolder?.Substring(projectLanguageFolder.Length).Trim('\\') + '\\';

				var fileBasedProject = backTranslationProjectStudio as FileBasedProject;
				var fileBasedProjectInfo = fileBasedProject?.GetProjectInfo();

				var projectFiles = fileBasedProject?.GetTargetLanguageFiles(fileBasedProjectInfo.TargetLanguages.FirstOrDefault());

				var projectFile = projectFiles?.FirstOrDefault(a =>
					string.Compare(a.Name, fileName, StringComparison.CurrentCultureIgnoreCase) == 0 &&
					string.Compare(a.Folder.Trim('\\') + '\\', folderPathInProject, StringComparison.CurrentCultureIgnoreCase) == 0);

				var data = sdlxliffReader.ReadFile(fileBasedProjectInfo?.Id.ToString(), projectFile?.Id.ToString(), file, targetLanguage);
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
					sourceFiles.Add(sourceFile);
				}
				else if (!TaskContext.BackTranslationOptions.OverwriteExistingBackTranslations)
				{
					sourceFiles.Remove(existingSourceFile);
					sourceFiles.Add(sourceFile);
				}
			}
		}

		private List<SourceFile> GetSelectedSourceFiles(string languageName, IReadOnlyCollection<FileData> fileDataList, IXliffWriter xliffWriter)
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
					FileName = Path.GetFileName(xliffFilePath)
				});
			}

			return sourceFiles;
		}

		private IProject CloseBackTranslationProject(string languageName)
		{
			var backTranslationProject = TaskContext.Project.BackTranslationProjects.FirstOrDefault(a =>
				string.Compare(a.SourceLanguage.CultureInfo.Name, languageName, StringComparison.CurrentCultureIgnoreCase) == 0);
			if (backTranslationProject != null)
			{
				var backTranslationProjectStudio = _controllers.ProjectsController.GetProjects()
					.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == backTranslationProject.Id);

				if (backTranslationProjectStudio != null)
				{
					_controllers.ProjectsController.Close(backTranslationProjectStudio);
				}
				else
				{
					var projectPaths = Directory.GetFiles(backTranslationProject.Path, "*.sdlproj", SearchOption.TopDirectoryOnly);
					if (projectPaths.Any())
					{
						backTranslationProjectStudio = _controllers.ProjectsController.Add(projectPaths).FirstOrDefault();

						_controllers.ProjectsController.Close(backTranslationProjectStudio);
					}
				}

				return backTranslationProjectStudio;
			}

			return null;
		}

		private void UpdateStudio(IEnumerable<TaskContext> taskContexts)
		{
			foreach (var taskContext in taskContexts)
			{
				_projectAutomationService.ActivateProject(taskContext.FileBasedProject);
				_controllers.ProjectsController.RefreshProjects();

				UpdateProjectSettingsBundle(taskContext.FileBasedProject);
				_controllers.TranscreateController.UpdateBackTranslationProjectData(TaskContext.Project.Id, taskContext);
			}

			_controllers.TranscreateController.InvalidateProjectsContainer();
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
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private TaskContext CreateBackTranslationTaskContext(FileBasedProject newStudioProject,
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

					var paragraphMap = GetParagraphMap(sdlxliffReader, projectFile.ProjectId, projectFile.FileId,
						projectFile.Location, projectFile.TargetLanguage);
					AlignParagraphIds(fileData.Data, paragraphMap.Keys.ToList());

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

			return taskContext;
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

		private Dictionary<string, List<string>> GetParagraphMap(SdlxliffReader sdlxliffReader, string projectId, string fileId, string path, string targetLanguage)
		{
			var xliffData = sdlxliffReader.ReadFile(projectId, fileId, path, targetLanguage);
			return GetParagraphMap(xliffData);
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

		private void UpdateProjectSettingsBundle(FileBasedProject project)
		{
			var settingsBundle = project.GetSettings();
			var sdlTranscreateProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();
			var projectFiles = new List<SDLTranscreateProjectFile>();
			sdlTranscreateProject.ProjectFilesJson.Value = JsonConvert.SerializeObject(projectFiles);
			project.UpdateSettings(sdlTranscreateProject.SettingsBundle);

			var sdlBackTranslateProjects = settingsBundle.GetSettingsGroup<SDLTranscreateBackProjects>();
			var backProjects = new List<SDLTranscreateBackProject>();
			sdlBackTranslateProjects.BackProjectsJson.Value = JsonConvert.SerializeObject(backProjects);
			project.UpdateSettings(sdlTranscreateProject.SettingsBundle);

			project.Save();
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


		private string GetBackTranslationIconPath()
		{
			var iconPath = Path.Combine(_pathInfo.ApplicationIconsFolderPath, "SDLBackTranslation.ico");
			if (!File.Exists(iconPath))
			{
				using (var fs = new FileStream(iconPath, FileMode.Create))
				{
					PluginResources.back_translation_small.Save(fs);
				}
			}

			return iconPath;
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
			_logReport.AppendLine("End Process: Export " + FormatDateTime(DateTime.UtcNow));

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
			_logReport.AppendLine("Start Process: Export " + FormatDateTime(DateTime.UtcNow));
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
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_XliffSupport, TaskContext.ExportOptions.XliffSupport));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_WorkingFolder, TaskContext.WorkingFolder));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_IncludeTranslations, TaskContext.ExportOptions.IncludeTranslations));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_CopySourceToTarget, TaskContext.ExportOptions.CopySourceToTarget));
			if (TaskContext.ExportOptions.ExcludeFilterIds.Count > 0)
			{
				_logReport.AppendLine(indent + string.Format(PluginResources.Label_ExcludeFilters, GetFitlerItemsString(TaskContext.ExportOptions.ExcludeFilterIds)));
			}

			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Files);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TotalFiles, TaskContext.ProjectFiles.Count));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ExportFiles, TaskContext.ProjectFiles.Count(a => a.Selected)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()));
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

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
