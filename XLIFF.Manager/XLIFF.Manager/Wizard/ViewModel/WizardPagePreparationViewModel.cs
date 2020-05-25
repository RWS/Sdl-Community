using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Sdl.Community.XLIFF.Manager.Converters.XLIFF;
using Sdl.Community.XLIFF.Manager.Converters.XLIFF.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPagePreparationViewModel : WizardPageViewModelBase
	{
		private ObservableCollection<JobProcess> _jobProcesses;	

		public WizardPagePreparationViewModel(Window owner, UserControl view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
		{		
			IsValid = true;
			InitializeJobProcessList();
			PropertyChanged += WizardPagePreparationViewModel_PropertyChanged;					
		}

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

		public override string DisplayName => "Preparation";

		public sealed override bool IsValid { get; set; }

		private void InitializeJobProcessList()
		{
			JobProcesses = new ObservableCollection<JobProcess>
			{
				new JobProcess
				{
					Name = "Preparation"
				},
				new JobProcess
				{
					Name = "Export"
				},
				new JobProcess
				{
					Name = "Finalize"
				}
			};
		}		

		private string TimeStamp { get; set; }

		public SolidColorBrush TextMessageBrush { get; set; }

		private async void StartProcessing()
		{
			TimeStamp = GetDateToString();

			//_logger.Info("Start Process: XLIFF.Manager.Export");

			var success = true;
			var job = JobProcesses.FirstOrDefault(a => a.Name == "Preparation");
			if (job != null)
			{
				success = Preparation(job);
			}

			if (success)
			{
				job = JobProcesses.FirstOrDefault(a => a.Name == "Export");
				if (job != null)
				{					

					success = await Export(job);
				}
			}

			if (success)
			{

				job = JobProcesses.FirstOrDefault(a => a.Name == "Finalize");
				if (job != null)
				{
					

					success = Finalize(job);
				}
			}
	
			if (success)
			{
				TextMessage = "Successful";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#017701");
				IsComplete = true;
			}
			else
			{
				TextMessage = "Unsuccessful";
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
				OnPropertyChanged(nameof(TextMessage));
				OnPropertyChanged(nameof(TextMessageBrush));
				jobProcess.Status = JobProcess.ProcessStatus.Running;
				Refresh();

				// clear the warnings
				System.Threading.Thread.Sleep(3000);
				

				jobProcess.Status = JobProcess.ProcessStatus.Completed;
				//Refresh();
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
				OnPropertyChanged(nameof(TextMessage));
				OnPropertyChanged(nameof(TextMessageBrush));
				jobProcess.Status = JobProcess.ProcessStatus.Running;
				Refresh();

				var project = WizardContext.ProjectFileModels[0].ProjectModel;
				var workingFolder = GetWorkingFolder();

				var sdlXliffParser = new FileParser();
				var xliffWriter = new Writer();

				var selectedLanguages = GetSelectedLanguages();

				foreach (var cultureInfo in selectedLanguages)
				{
					var languageFolder = GetLanguageFolder(workingFolder, cultureInfo);

					var targetFiles = GetSelectedTargetFiles(cultureInfo);
					foreach (var targetFile in targetFiles)
					{
						var outputFilePath = Path.Combine(languageFolder, targetFile.Name + ".xliff");

						var xliffData = sdlXliffParser.ParseFile(project.Id, targetFile.Location, WizardContext.CopySourceToTarget);

						xliffWriter.CreateXliffFile(xliffData, outputFilePath, WizardContext.XLIFFSupport, WizardContext.IncludeTranslations);
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

		private string GetLanguageFolder(string workingFolder, CultureInfo cultureInfo)
		{
			var languageFolder = Path.Combine(workingFolder, cultureInfo.Name);
			if (!Directory.Exists(languageFolder))
			{
				Directory.CreateDirectory(languageFolder);
			}

			return languageFolder;
		}

		private string GetWorkingFolder()
		{
			var workingFolder = Path.Combine(WizardContext.OutputFolder, TimeStamp);
			if (!Directory.Exists(workingFolder))
			{
				Directory.CreateDirectory(workingFolder);
			}

			return workingFolder;
		}

		private bool Finalize(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				//_logger.Info("Phase: Finalize");


				TextMessage = "Finalizing procedures...";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
				OnPropertyChanged(nameof(TextMessage));
				OnPropertyChanged(nameof(TextMessageBrush));
				jobProcess.Status = JobProcess.ProcessStatus.Running;
				Refresh();


				// clear the warnings
				System.Threading.Thread.Sleep(3000);


				jobProcess.Status = JobProcess.ProcessStatus.Completed;
				//Refresh();

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

		private static string GetDateToString()
		{
			var now = DateTime.Now;
			var value = now.Year
			            + "" + now.Month.ToString().PadLeft(2, '0')
			            + "" + now.Day.ToString().PadLeft(2, '0')
			            + "" + now.Hour.ToString().PadLeft(2, '0')
			            + "" + now.Minute.ToString().PadLeft(2, '0')
			            + "" + now.Second.ToString().PadLeft(2, '0');

			return value;
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
