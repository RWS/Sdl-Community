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
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Wizard.View;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportPreparationViewModel : WizardPageViewModelBase, IDisposable
	{
		private const string ForegroundSuccess = "#017701";
		private const string ForegroundError = "#7F0505";
		private const string ForegroundProcessing = "#0096D6";

		private readonly SegmentBuilder _segmentBuilder;
		private readonly PathInfo _pathInfo;
		private List<JobProcess> _jobProcesses;
		private ICommand _viewExceptionCommand;
		private ICommand _openFolderInExplorerCommand;
		private SolidColorBrush _textMessageBrush;
		private string _textMessage;
		private StringBuilder _logReport;

		public WizardPageImportPreparationViewModel(Window owner, object view, WizardContext wizardContext,
			SegmentBuilder segmentBuilder, PathInfo pathInfo) : base(owner, view, wizardContext)
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
			if (Directory.Exists(WizardContext.WorkingFolder))
			{
				Process.Start(WizardContext.WorkingFolder);
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
					Name = PluginResources.JobProcess_Export
				},
				new JobProcess
				{
					Name = PluginResources.JobProcess_Finalize
				}
			};
		}

		private async void StartProcessing()
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
				job = JobProcesses.FirstOrDefault(a => a.Name == PluginResources.JobProcess_Export);
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

			SaveLogReport();
		}
	
		private async Task<bool> Preparation(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Preparation - Started " + FormatDateTime(DateTime.Now));

				TextMessage = PluginResources.WizardMessage_Initializing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;
				
				Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.Send);

				_logReport.AppendLine("Phase: Preparation - Complete " + FormatDateTime(DateTime.Now));
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

		private async Task<bool> Import(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Import - Started " + FormatDateTime(DateTime.Now));

				TextMessage = PluginResources.WizardMessage_ImportingFromFormat;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.Send);


				var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
				var sdlxliffWriter = new SdlxliffWriter(fileTypeManager, _segmentBuilder);

				var sniffer = new XliffSniffer();
				var xliffReader = new XliffReder(sniffer, _segmentBuilder);

				var targetLanguages = WizardContext.ProjectFiles.Where(a => a.Selected &&
									!string.IsNullOrEmpty(a.XliffFilePath) && File.Exists(a.XliffFilePath))
									.Select(a => a.TargetLanguage.CultureInfo).Distinct();

				foreach (var targetLanguage in targetLanguages)
				{
					var languageFolder = GetLanguageFolder(targetLanguage);

					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, targetLanguage.DisplayName));

					var targetLanguageFiles = WizardContext.ProjectFiles.Where(a => a.Selected &&
									!string.IsNullOrEmpty(a.XliffFilePath) && File.Exists(a.XliffFilePath) &&
									Equals(a.TargetLanguage.CultureInfo, targetLanguage));

					foreach (var targetLanguageFile in targetLanguageFiles)
					{
						var xliffFolder = GetXliffFolder(languageFolder, targetLanguageFile);
						var xliffFilePath = Path.Combine(xliffFolder, targetLanguageFile.Name + ".xliff");
						var sdlXliffBackup = Path.Combine(xliffFolder, targetLanguageFile.Name);

						_logReport.AppendLine(string.Format(PluginResources.label_SdlXliffFile, targetLanguageFile.Location));
						if (WizardContext.ImportBackupFiles)
						{
							_logReport.AppendLine(string.Format(PluginResources.Label_BackupFile, sdlXliffBackup));
						}
						_logReport.AppendLine(string.Format(PluginResources.label_XliffFile, targetLanguageFile.XliffFilePath));
						_logReport.AppendLine(string.Format(PluginResources.Label_ArchiveFile, xliffFilePath));

						CreateBackupFile(targetLanguageFile, sdlXliffBackup);
						CreateArchiveFile(targetLanguageFile, xliffFilePath);

						var outputFilePath = Path.GetTempFileName();

						var xliff = xliffReader.ReadXliff(xliffFilePath);
						success = sdlxliffWriter.UpdateFile(xliff, sdlXliffBackup, outputFilePath,
							WizardContext.ImportOverwriteTranslations, WizardContext.ImportConfirmationStatus, WizardContext.ImportOriginSystem);

						_logReport.AppendLine(string.Format(PluginResources.Label_Success, success));
						_logReport.AppendLine();

						if (success)
						{
							CompleteImportProcedures(targetLanguageFile, outputFilePath);
						}
						else
						{
							UndoImportProcedures(targetLanguageFile, outputFilePath, sdlXliffBackup);
						}
					}
				}

				_logReport.AppendLine("Phase: Import - Completed " + FormatDateTime(DateTime.Now));

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
				_logReport.AppendLine("Phase: Finalize - Started " + FormatDateTime(DateTime.Now));

				TextMessage = PluginResources.WizardMessage_Finalizing;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.Send);

				_logReport.AppendLine("Phase: Finalize - Completed " + FormatDateTime(DateTime.Now));
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

		private void FinalizeJobProcesses(bool success)
		{
			_logReport.AppendLine();
			_logReport.AppendLine("End Process: Import " + FormatDateTime(DateTime.Now));

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

		private void WriteLogReportHeader()
		{
			_logReport = new StringBuilder();
			_logReport.AppendLine("Start Process: Import " + FormatDateTime(DateTime.Now));
			_logReport.AppendLine();

			var project = WizardContext.ProjectFiles[0].Project;

			var indent = "   ";
			_logReport.AppendLine(PluginResources.Label_Project);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Id, project.Id));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Name, project.Name));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Location, project.Path));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Created,
									  project.Created.ToString(CultureInfo.InvariantCulture)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_DueDate,
									  project.DueDate.ToString(CultureInfo.InvariantCulture)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_SourceLanguage,
									  project.SourceLanguage.CultureInfo.DisplayName));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TargetLanguages,
									  GetProjectTargetLanguagesString(project)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ProjectType, project.ProjectType));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Customer, project.Customer?.Name));

			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Options);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_BackupFiles, WizardContext.ImportBackupFiles));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_OverwriteExistingTranslations,
									  WizardContext.ImportOverwriteTranslations));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_OriginSystem, WizardContext.ImportOriginSystem));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ConfirmationStatus,
									  WizardContext.ImportConfirmationStatus.ToString()));


			_logReport.AppendLine();
			_logReport.AppendLine(PluginResources.Label_Files);
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_TotalFiles, WizardContext.ProjectFiles.Count));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_ImportFiles,
									  WizardContext.ProjectFiles.Count(a => a.Selected)));
			_logReport.AppendLine(indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()));
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

		private string GetLanguageFolder(CultureInfo targetLanguage)
		{
			var languageFolder = WizardContext.GetLanguageFolder(targetLanguage);
			if (!Directory.Exists(languageFolder))
			{
				Directory.CreateDirectory(languageFolder);
			}

			return languageFolder;
		}

		private static void UndoImportProcedures(ProjectFile targetLanguageFile, string outputFilePath, string sdlXliffBackup)
		{
			if (File.Exists(outputFilePath))
			{
				File.Delete(outputFilePath);
			}

			if (File.Exists(sdlXliffBackup))
			{
				if (File.Exists(targetLanguageFile.Location))
				{
					File.Delete(targetLanguageFile.Location);
				}

				File.Copy(sdlXliffBackup, targetLanguageFile.Location, true);
			}

			throw new Exception("Error occurred while updating the SDLXLIFF");
		}

		private static void CompleteImportProcedures(ProjectFile targetLanguageFile, string outputFilePath)
		{
			if (File.Exists(targetLanguageFile.Location))
			{
				File.Delete(targetLanguageFile.Location);
			}

			File.Copy(outputFilePath, targetLanguageFile.Location, true);

			File.Delete(outputFilePath);
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

		private void CreateArchiveFile(ProjectFile targetLanguageFile, string xliffFilePath)
		{
			if (File.Exists(xliffFilePath))
			{
				File.Delete(xliffFilePath);
			}

			File.Copy(targetLanguageFile.XliffFilePath, xliffFilePath, true);
		}

		private void CreateBackupFile(ProjectFile targetLanguageFile, string sdlXliffBackup)
		{
			if (File.Exists(sdlXliffBackup))
			{
				File.Delete(sdlXliffBackup);
			}

			File.Copy(targetLanguageFile.Location, sdlXliffBackup, true);
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
			foreach (var cultureInfo in selected.Select(a => a.TargetLanguage.CultureInfo).Distinct())
			{
				selectedLanguages += (string.IsNullOrEmpty(selectedLanguages) ? string.Empty : ", ") +
									 cultureInfo.DisplayName;
			}

			return selectedLanguages;
		}

		private IEnumerable<ProjectFile> GetSelectedTargetFiles(CultureInfo cultureInfo)
		{
			var selected = WizardContext.ProjectFiles.Where(a => a.Selected);
			var targetFiles = selected.Where(a => Equals(a.TargetLanguage.CultureInfo, cultureInfo));
			return targetFiles;
		}

		private IEnumerable<CultureInfo> GetSelectedLanguages()
		{
			var selected = WizardContext.ProjectFiles.Where(a => a.Selected);
			var selectedLanguages = selected.Select(a => a.TargetLanguage.CultureInfo).Distinct();
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
