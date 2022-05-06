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
using NLog;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Trados.Transcreate.Commands;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.MSOffice;
using Trados.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.Model;
using Trados.Transcreate.Wizard.View;

namespace Trados.Transcreate.Wizard.ViewModel.Import
{
	public class WizardPageImportPreparationViewModel : WizardPageViewModelBase, IDisposable
	{
		private const string ForegroundSuccess = "#017701";
		private const string ForegroundError = "#7F0505";
		private const string ForegroundProcessing = "#0096D6";

		private readonly FileTypeSupport.SDLXLIFF.SegmentBuilder _segmentBuilder;
		private readonly PathInfo _pathInfo;
		private List<JobProcess> _jobProcesses;
		private ICommand _viewExceptionCommand;
		private ICommand _openFolderInExplorerCommand;
		private SolidColorBrush _textMessageBrush;
		private string _textMessage;
		private StringBuilder _logReport;

		public WizardPageImportPreparationViewModel(Window owner, object view, TaskContext taskContext,
			FileTypeSupport.SDLXLIFF.SegmentBuilder segmentBuilder, PathInfo pathInfo) : base(owner, view, taskContext)
		{
			_segmentBuilder = segmentBuilder;
			_pathInfo = pathInfo;

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
					Name = PluginResources.JobProcess_Import
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
					job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_Import);
					if (job != null)
					{
						success = await Import(job);
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
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
			}
			finally
			{
				IsProcessing = false;
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
				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> Import(JobProcess jobProcess)
		{
			var success = true;
			var importFiles = new List<ImportFile>();

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Import - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_ImportingFromFormat;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				var tokenVisitor = new TokenVisitor();
				var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
				var segmentBuilder = new SegmentBuilder();
				var processor = new ImportProcessor(fileTypeManager, tokenVisitor, TaskContext.ImportOptions, TaskContext.AnalysisBands, segmentBuilder);

				var targetLanguages = TaskContext.ProjectFiles.Where(
									a => a.Selected &&
									!string.IsNullOrEmpty(a.ExternalFilePath) &&
									File.Exists(a.ExternalFilePath))
									.Select(a => a.TargetLanguage).Distinct();

				foreach (var targetLanguage in targetLanguages)
				{
					var languageFolder = GetLanguageFolder(targetLanguage);

					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, targetLanguage));

					var targetLanguageFiles = TaskContext.ProjectFiles.Where(
												a => a.Selected &&
												!string.IsNullOrEmpty(a.ExternalFilePath) &&
												File.Exists(a.ExternalFilePath) &&
												string.Compare(a.TargetLanguage, targetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0);

					foreach (var targetLanguageFile in targetLanguageFiles)
					{
						var folder = GetXliffFolder(languageFolder, targetLanguageFile);
						var archiveFile = Path.Combine(folder, targetLanguageFile.Name + ".docx");
						var sdlXliffBackupFile = Path.Combine(folder, targetLanguageFile.Name);

						_logReport.AppendLine(string.Format(PluginResources.Label_SdlXliffFile, targetLanguageFile.Location));
						if (TaskContext.ImportOptions.BackupFiles)
						{
							_logReport.AppendLine(string.Format(PluginResources.Label_BackupFile, sdlXliffBackupFile));
						}

						_logReport.AppendLine(string.Format(PluginResources.Label_ImportFile, targetLanguageFile.ExternalFilePath));
						_logReport.AppendLine(string.Format(PluginResources.Label_ArchiveFile, archiveFile));

						CreateBackupFile(targetLanguageFile.Location, sdlXliffBackupFile);
						CreateArchiveFile(targetLanguageFile.ExternalFilePath, archiveFile);

						var sdlXliffImportFile = Path.GetTempFileName();

						var importFile = new ImportFile
						{
							SdlXliffFile = targetLanguageFile.Location,
							SdlXliffBackupFile = sdlXliffBackupFile,
							SdlXliffImportFile = sdlXliffImportFile,
							ImportFilePath = targetLanguageFile.ExternalFilePath,
							ArchiveFilePath = archiveFile
						};
						importFiles.Add(importFile);

						success = processor.ImportUpdatedFile(targetLanguageFile.Location, 
							sdlXliffImportFile, targetLanguageFile.ExternalFilePath, targetLanguage);
						if (success)
						{
							targetLanguageFile.Date = TaskContext.DateTimeStamp;
							targetLanguageFile.Action = TaskContext.Action;
							targetLanguageFile.WorkFlow = TaskContext.WorkFlow;
							targetLanguageFile.Status = Enumerators.Status.Success;
							targetLanguageFile.Report = string.Empty;
							targetLanguageFile.ExternalFilePath = archiveFile;
							targetLanguageFile.ConfirmationStatistics = processor.ConfirmationStatistics;
							targetLanguageFile.TranslationOriginStatistics = processor.TranslationOriginStatistics;
						}

						var activityFile = new ProjectFileActivity
						{
							ProjectFileId = targetLanguageFile.FileId,
							ActivityId = Guid.NewGuid().ToString(),
							Action = TaskContext.Action,
							WorkFlow = TaskContext.WorkFlow,
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

				_logReport.AppendLine("Phase: Import - Completed " + FormatDateTime(DateTime.UtcNow));

				TaskContext.Completed = true;
				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

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
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				_logReport.AppendLine("Phase: Finalize - Completed " + FormatDateTime(DateTime.UtcNow));
				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private void FinalizeJobProcesses(bool success)
		{
			_logReport.AppendLine();
			_logReport.AppendLine("End Process: Import " + FormatDateTime(DateTime.UtcNow));

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

		private void WriteLogReportHeader()
		{
			_logReport = new StringBuilder();
			_logReport.AppendLine("Start Process: Import " + FormatDateTime(DateTime.UtcNow));
			_logReport.AppendLine();

			var project = TaskContext.ProjectFiles[0].Project;

			var indent = "   ";
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
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_BackupFiles, TaskContext.ImportOptions.BackupFiles));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_OverwriteExistingTranslations, TaskContext.ImportOptions.OverwriteTranslations));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_OriginSystem, TaskContext.ImportOptions.OriginSystem));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_StatusTranslationsUpdated, GetConfirmationStatusName(TaskContext.ImportOptions.StatusTranslationUpdatedId)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_StatusTranslationsNotUpdated, GetConfirmationStatusName(TaskContext.ImportOptions.StatusTranslationNotUpdatedId)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_StatusSegmentsNotImported, GetConfirmationStatusName(TaskContext.ImportOptions.StatusSegmentNotImportedId)));
			if (TaskContext.ImportOptions.ExcludeFilterIds.Count > 0)
			{
				_logReport.AppendLine(indent + string.Format(PluginResources.Label_ExcludeFilters, GetFitlerItemsString(TaskContext.ImportOptions.ExcludeFilterIds)));
			}


			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Files);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TotalFiles, TaskContext.ProjectFiles.Count));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ImportFiles, TaskContext.ProjectFiles.Count(a => a.Selected)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()));
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

		private string GetLanguageFolder(string name)
		{
			var languageFolder = TaskContext.GetLanguageFolder(name);
			if (!Directory.Exists(languageFolder))
			{
				Directory.CreateDirectory(languageFolder);
			}

			return languageFolder;
		}

		private static string GetXliffFolder(string languageFolder, ProjectFile targetLanguageFile)
		{
			var xliffFolder = Path.Combine(languageFolder, targetLanguageFile.Path.TrimStart('\\'));
			if (!Directory.Exists(xliffFolder))
			{
				Directory.CreateDirectory(xliffFolder);
			}

			return xliffFolder;
		}

		private void CreateArchiveFile(string xliffFile, string xliffArchiveFile)
		{
			if (File.Exists(xliffArchiveFile))
			{
				File.Delete(xliffArchiveFile);
			}

			File.Copy(xliffFile, xliffArchiveFile, true);
		}

		private void CreateBackupFile(string sdlXliffFile, string sdlXliffBackup)
		{
			if (File.Exists(sdlXliffBackup))
			{
				File.Delete(sdlXliffBackup);
			}

			File.Copy(sdlXliffFile, sdlXliffBackup, true);
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

		private string GetConfirmationStatusName(string id)
		{
			return string.IsNullOrEmpty(id) ? "[none]" : id;
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
