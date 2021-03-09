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
using Trados.Transcreate.Model;
using Trados.Transcreate.Wizard.View;

namespace Trados.Transcreate.Wizard.ViewModel.Export
{
	public class WizardPageExportPreparationViewModel : WizardPageViewModelBase
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

		public WizardPageExportPreparationViewModel(Window owner, UserControl view, TaskContext taskContext,
			FileTypeSupport.SDLXLIFF.SegmentBuilder segmentBuilder, PathInfo pathInfo)
			: base(owner, view, taskContext)
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
						success = await Export(job);
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
				LogManager.GetCurrentClassLogger().Error(ex);
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				_logReport.AppendLine();
				_logReport.AppendLine(string.Format(PluginResources.Label_ExceptionMessage, ex.Message));
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> Export(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Export - Started " + FormatDateTime(DateTime.UtcNow));

				TextMessage = PluginResources.WizardMessage_ConvertingToFormat;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ForegroundProcessing);
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Refresh();

				var tokenVisitor = new TokenVisitor();
				var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
				var processor = new ExportProcessor(TaskContext.Project.Id, fileTypeManager, tokenVisitor, TaskContext.ExportOptions, TaskContext.AnalysisBands);
				var selectedLanguages = GetSelectedLanguages();

				foreach (var name in selectedLanguages)
				{
					_logReport.AppendLine();
					_logReport.AppendLine(string.Format(PluginResources.Label_Language, name));

					var languageFolder = GetLanguageFolder(name);
					var targetFiles = GetSelectedTargetFiles(name);
					foreach (var targetFile in targetFiles)
					{
						var folder = GetXliffFolder(languageFolder, targetFile);
						var outputFilePath = Path.Combine(folder, targetFile.Name + ".docx");

						_logReport.AppendLine(string.Format(PluginResources.Label_SdlXliffFile, targetFile.Location));
						_logReport.AppendLine(string.Format(PluginResources.Label_XliffFile, outputFilePath));

						var exported  = processor.ExportFile(targetFile.Location, outputFilePath, targetFile.TargetLanguage);

						_logReport.AppendLine(string.Format(PluginResources.Label_Success, exported));
						_logReport.AppendLine();

						if (exported)
						{
							targetFile.Date = TaskContext.DateTimeStamp;
							targetFile.Action = TaskContext.Action;
							targetFile.WorkFlow = TaskContext.WorkFlow;
							targetFile.Status = Enumerators.Status.Success;
							targetFile.ExternalFilePath = outputFilePath;
							targetFile.ConfirmationStatistics = processor.ConfirmationStatistics;
							targetFile.TranslationOriginStatistics = processor.TranslationOriginStatistics;
						}

						var activityFile = new ProjectFileActivity
						{
							ProjectFileId = targetFile.FileId,
							ActivityId = Guid.NewGuid().ToString(),
							Action = TaskContext.Action,
							WorkFlow = TaskContext.WorkFlow,
							Status = exported ? Enumerators.Status.Success : Enumerators.Status.Error,
							Date = targetFile.Date,
							Name = Path.GetFileName(targetFile.ExternalFilePath),
							Path = Path.GetDirectoryName(targetFile.ExternalFilePath),
							ProjectFile = targetFile,
							ConfirmationStatistics = targetFile.ConfirmationStatistics,
							TranslationOriginStatistics = targetFile.TranslationOriginStatistics
						};

						targetFile.ProjectFileActivities.Add(activityFile);
					}
				}

				_logReport.AppendLine();
				_logReport.AppendLine("Phase: Export - Completed " + FormatDateTime(DateTime.UtcNow));

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
