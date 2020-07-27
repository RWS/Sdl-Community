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
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Writers;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Service;
using Sdl.Community.Transcreate.Wizard.View;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
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
					Name = PluginResources.JobProcess_Finalize
				}
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
				_logReport.AppendLine(string.Format(PluginResources.label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> ConvertProjectFiles(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Convert Project Files - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_Initializing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				var project = WizardContext.ProjectFiles[0].Project;
				
				var sdlxliffReader = new SdlxliffReader(_segmentBuilder,
					WizardContext.ExportOptions, WizardContext.AnalysisBands);
				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);

				var selectedLanguages = GetSelectedLanguages();
				foreach (var name in selectedLanguages)
				{
					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, name));

					var languageFolder = GetLanguageFolder(name);
					var targetFiles = GetSelectedTargetFiles(name);
					foreach (var targetFile in targetFiles)
					{
						var xliffFolder = GetXliffFolder(languageFolder, targetFile);
						var xliffFilePath = Path.Combine(xliffFolder, targetFile.Name.Substring(0, targetFile.Name.Length - ".sdlxliff".Length) + ".xliff");
						
						_logReport.AppendLine(string.Format(PluginResources.label_SdlXliffFile, targetFile.Location));
						_logReport.AppendLine(string.Format(PluginResources.label_XliffFile, xliffFilePath));
						
						var xliffData = sdlxliffReader.ReadFile(project.Id, targetFile.Location);
						var exported = xliffWriter.WriteFile(xliffData, xliffFilePath, WizardContext.ExportOptions.IncludeTranslations);

						_logReport.AppendLine(string.Format(PluginResources.Label_Success, exported));
						_logReport.AppendLine();

						if (exported)
						{
							targetFile.Date = WizardContext.DateTimeStamp;
							targetFile.Action = Enumerators.Action.Convert;
							targetFile.Status = Enumerators.Status.Success;
							targetFile.XliffFilePath = xliffFilePath;
							targetFile.ConfirmationStatistics = sdlxliffReader.ConfirmationStatistics;
							targetFile.TranslationOriginStatistics = sdlxliffReader.TranslationOriginStatistics;
						}

						var activityFile = new ProjectFileActivity
						{
							ProjectFileId = targetFile.FileId,
							ActivityId = Guid.NewGuid().ToString(),
							Action = Enumerators.Action.Convert,
							Status = exported ? Enumerators.Status.Success : Enumerators.Status.Error,
							Date = targetFile.Date,
							Name = Path.GetFileName(targetFile.XliffFilePath),
							Path = Path.GetDirectoryName(targetFile.XliffFilePath),
							ProjectFile = targetFile,
							ConfirmationStatistics = targetFile.ConfirmationStatistics,
							TranslationOriginStatistics = targetFile.TranslationOriginStatistics
						};

						targetFile.ProjectFileActivities.Add(activityFile);

						if (!exported)
						{
							throw new Exception(string.Format(PluginResources.ErrorMessage_ConvertingFile, targetFile.Location));
						}
					}
				}


				_logReport.AppendLine("Phase: Convert Project Files - Complete " + FormatDateTime(DateTime.UtcNow));
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

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Create Transcreate Project - Started " + FormatDateTime(DateTime.UtcNow));

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

				var newProject = _projectAutomationService.CreateTranscreateProject(selectedProject, WizardContext.ProjectFiles);
				var newProjectInfo = newProject.GetProjectInfo();


				if (WizardContext.ConvertOptions.CloseProjectOnComplete)
				{
					_controllers.ProjectsController.Close(selectedProject);
					_controllers.ProjectsController.RefreshProjects();
				}

				_controllers.ProjectsController.Open(newProject);

				// New Wizard Context - based on newProject
				WizardContext.Project = _projectAutomationService.GetProject(newProject, null);
				WizardContext.ProjectFiles = WizardContext.Project.ProjectFiles;
				WizardContext.AnalysisBands = _projectAutomationService.GetAnalysisBands(selectedProject);
				WizardContext.LocalProjectFolder = newProjectInfo.LocalProjectFolder;
				WizardContext.TransactionFolder = WizardContext.GetDefaultTransactionPath();

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Create Transcreate Project - Completed " + FormatDateTime(DateTime.UtcNow));

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




				//foreach (var projectFile in WizardContext.ProjectFiles)
				//{
				//	//projectFile.
				//}

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

		private void CreateBackupFile(string sdlXliffFile, string sdlXliffBackup)
		{
			if (File.Exists(sdlXliffBackup))
			{
				File.Delete(sdlXliffBackup);
			}

			File.Copy(sdlXliffFile, sdlXliffBackup, true);
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
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ProjectBackup, WizardContext.ProjectBackupPath));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_MaxAlternativeTranslations, WizardContext.ConvertOptions.MaxAlternativeTranslations));

			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Files);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TotalFiles, WizardContext.ProjectFiles.Count));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_SelectedFiles, WizardContext.ProjectFiles.Count(a => a.Selected)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()));
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

		private string GetSelectedLanguagesString()
		{
			var selected = WizardContext.ProjectFiles.Where(a => a.Selected);

			var selectedLanguages = string.Empty;
			foreach (var name in selected.Select(a => a.TargetLanguage).Distinct())
			{
				selectedLanguages += (string.IsNullOrEmpty(selectedLanguages) ? string.Empty : ", ") + name;
			}

			return selectedLanguages;
		}

		private IEnumerable<ProjectFile> GetSelectedTargetFiles(string name)
		{
			var selected = WizardContext.ProjectFiles.Where(a => a.Selected);
			var targetFiles = selected.Where(a => Equals(a.TargetLanguage, name));
			return targetFiles;
		}

		private IEnumerable<string> GetSelectedLanguages()
		{
			var selected = WizardContext.ProjectFiles.Where(a => a.Selected);
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
