using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Writers;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageExportPreparationViewModel : WizardPageViewModelBase
	{
		private List<JobProcess> _jobProcesses;
		private ICommand _openFolderInExplorerCommand;
		private SolidColorBrush _textMessageBrush;
		private string _textMessage;

		public WizardPageExportPreparationViewModel(Window owner, UserControl view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			IsValid = true;
			InitializeJobProcessList();
			PropertyChanged += WizardPagePreparationViewModel_PropertyChanged;
		}

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
			//_logger.Info("Start Process: XLIFF.Manager.Export");

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

			//_logger.Info("End Process: XLIFF.Manager.Export" + Environment.NewLine + Environment.NewLine);
		}

		private async Task<bool> Preparation(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logger.Info("Phase: Preparation");

				TextMessage = "Initialzing procedures...";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				//Refresh();
				Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.Send);


				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				//_logger.Error(ex.Message);
			}

			return await Task.FromResult(success);
		}

		private async Task<bool> Export(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logger.Info("Phase: Export");

				TextMessage = "Converting to XLIFF format...";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
				jobProcess.Status = JobProcess.ProcessStatus.Running;
			
				Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.Send);

				var project = WizardContext.ProjectFileModels[0].ProjectModel;
				var sdlxliffReader = new SdlxliffReader();
				var xliffWriter = new XliffWriter(WizardContext.Support);

				var selectedLanguages = GetSelectedLanguages();

				foreach (var cultureInfo in selectedLanguages)
				{
					var languageFolder = WizardContext.GetLanguageFolder(cultureInfo);
					if (!Directory.Exists(languageFolder))
					{
						Directory.CreateDirectory(languageFolder);
					}

					var targetFiles = GetSelectedTargetFiles(cultureInfo);
					foreach (var targetFile in targetFiles)
					{
						targetFile.XliffFilePath = Path.Combine(languageFolder, targetFile.Name + ".xliff");

						var xliffData = sdlxliffReader.ReadFile(project.Id, targetFile.Location, WizardContext.CopySourceToTarget);

						var exported = xliffWriter.WriteFile(xliffData, targetFile.XliffFilePath, WizardContext.IncludeTranslations);
						if (exported)
						{
							targetFile.Status = Enumerators.Status.Success;
						}

						var activityFile = new ProjectFileActivity
						{
							ProjectFileId = targetFile.Id,
							Id = Guid.NewGuid().ToString(),
							Action = targetFile.Action,
							Status = exported ? Enumerators.Status.Success : Enumerators.Status.Error,
							Date = targetFile.Date,
							Path = Path.GetFileName(targetFile.XliffFilePath),
							Name = Path.GetDirectoryName(targetFile.XliffFilePath),
							Details = "TODO",
							ProjectFile = targetFile
						};

						targetFile.ProjectFileActivities.Add(activityFile);
					}
				}

				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				//_logger.Error(ex.Message);
			}

			return await Task.FromResult(success);
		}	

		private async Task<bool> Finalize(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logger.Info("Phase: Finalize");
				TextMessage = "Finalizing procedures...";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.Send);


				jobProcess.Status = JobProcess.ProcessStatus.Completed;
			}
			catch (Exception ex)
			{
				jobProcess.Errors.Add(ex);
				jobProcess.Status = JobProcess.ProcessStatus.Failed;
				success = false;

				//_logger.Error(ex.Message);
			}

			return await Task.FromResult(success);
		}

		private void FinalizeJobProcesses(bool success)
		{
			if (success)
			{
				TextMessage = PluginResources.Result_Successful;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#017701");
				IsComplete = true;
			}
			else
			{
				TextMessage = PluginResources.Result_Unsuccessful;
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#7F0505");
				IsComplete = false;
			}
		}

		private IEnumerable<ProjectFile> GetSelectedTargetFiles(CultureInfo cultureInfo)
		{
			var selected = WizardContext.ProjectFileModels.Where(a => a.Selected);
			var targetFiles = selected.Where(a => Equals(a.TargetLanguage.CultureInfo, cultureInfo));
			return targetFiles;
		}

		private IEnumerable<CultureInfo> GetSelectedLanguages()
		{
			var selected = WizardContext.ProjectFileModels.Where(a => a.Selected);
			var selectedLanguages = selected.Select(a => a.TargetLanguage.CultureInfo).Distinct();
			return selectedLanguages;
		}

		private void Refresh()
		{
			Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
		}

		private void WizardPagePreparationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					OnLoadView();
				}
				else
				{
					OnLeaveView();
				}
			}
		}

		private void OnLeaveView()
		{
		}

		private void OnLoadView()
		{
			Refresh();
			StartProcessing();
		}
	}
}
