using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Sdl.Community.XLIFF.Manager.Converters.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.Converters.XLIFF;
using Sdl.Community.XLIFF.Manager.Converters.XLIFF.Writers;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPagePreparationViewModel : WizardPageViewModelBase
	{
		private ObservableCollection<JobProcess> _jobProcesses;
		private ICommand _openFolderInExplorerCommand;

		public WizardPagePreparationViewModel(Window owner, UserControl view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
		{					
			IsValid = true;
			InitializeJobProcessList();
			PropertyChanged += WizardPagePreparationViewModel_PropertyChanged;					
		}

		public ICommand OpenFolderInExplorerCommand => _openFolderInExplorerCommand ?? (_openFolderInExplorerCommand = new CommandHandler(OpenFolderInExplorer));

		public ObservableCollection<JobProcess> JobProcesses
		{
			get { return _jobProcesses; }
			set
			{
				_jobProcesses = value;
				OnPropertyChanged(nameof(JobProcesses));
			}
		}

		public string TextMessage { get; set; }

		public SolidColorBrush TextMessageBrush { get; set; }

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
			JobProcesses = new ObservableCollection<JobProcess>
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
				success = Preparation(job);
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
					success = Finalize(job);
				}
			}
	
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

			OnPropertyChanged(nameof(TextMessage));
			OnPropertyChanged(nameof(TextMessageBrush));

			//_logger.Info("End Process: XLIFF.Manager.Export" + Environment.NewLine + Environment.NewLine);
		}

		private bool Preparation(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logger.Info("Phase: Preparation");

				TextMessage = "Initialzing procedures...";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
				jobProcess.Status = JobProcess.ProcessStatus.Running;


				OnPropertyChanged(nameof(TextMessage));
				OnPropertyChanged(nameof(TextMessageBrush));
				OnPropertyChanged(nameof(JobProcesses));

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

			return success;
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

				OnPropertyChanged(nameof(TextMessage));
				OnPropertyChanged(nameof(TextMessageBrush));				
				OnPropertyChanged(nameof(JobProcesses));

				//Refresh();
				Owner.Dispatcher.Invoke(delegate { }, DispatcherPriority.Send);

				var project = WizardContext.ProjectFileModels[0].ProjectModel;				
				var sdlXliffParser = new FileParser();
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
						var outputFilePath = Path.Combine(languageFolder, targetFile.Name + ".xliff");

						var xliffData = sdlXliffParser.ParseFile(project.Id, targetFile.Location, WizardContext.CopySourceToTarget);

						xliffWriter.CreateXliffFile(xliffData, outputFilePath, WizardContext.IncludeTranslations);
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

		private IEnumerable<ProjectFileModel> GetSelectedTargetFiles(CultureInfo cultureInfo)
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

		private bool Finalize(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logger.Info("Phase: Finalize");
				TextMessage = "Finalizing procedures...";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
				jobProcess.Status = JobProcess.ProcessStatus.Running;

				OnPropertyChanged(nameof(TextMessage));
				OnPropertyChanged(nameof(TextMessageBrush));
				OnPropertyChanged(nameof(JobProcesses));

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

			return success;
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
					LoadView();
				}
				else
				{
					LeaveView();
				}
			}
		}

		private void LeaveView()
		{
		}

		private void LoadView()
		{
			Refresh();
			StartProcessing();
		}
	}
}
