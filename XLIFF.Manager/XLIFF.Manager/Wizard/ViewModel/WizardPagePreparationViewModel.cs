using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using NLog.Fluent;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.ProjectAutomation.Settings;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPagePreparationViewModel : WizardPageViewModelBase
	{
		private ObservableCollection<JobProcess> _jobProcesses;
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

		private void StartProcessing()
		{		
			Logger.Info("Start Process: XLIFF.Manager.Export");

			var success = true;
			var job = JobProcesses.FirstOrDefault(a => a.Name == "Preparation");
			if (job != null)
			{
				TextMessage = "Initialzing procedures...";
				TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
				OnPropertyChanged(nameof(TextMessage));
				OnPropertyChanged(nameof(TextMessageBrush));
				job.Status = JobProcess.ProcessStatus.Running;
				Refresh();
	

				success = Preparation(job);
			}

			if (success)
			{
				job = JobProcesses.FirstOrDefault(a => a.Name == "Export");
				if (job != null)
				{
					TextMessage = "Converting to XLIFF format...";
					TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
					OnPropertyChanged(nameof(TextMessage));
					OnPropertyChanged(nameof(TextMessageBrush));

					job.Status = JobProcess.ProcessStatus.Running;
					Refresh();

					success = Export(job);
				}
			}

			if (success)
			{

				job = JobProcesses.FirstOrDefault(a => a.Name == "Finalize");
				if (job != null)
				{
					TextMessage = "Finalizing procedures...";
					TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0096D6");
					OnPropertyChanged(nameof(TextMessage));
					OnPropertyChanged(nameof(TextMessageBrush));
					job.Status = JobProcess.ProcessStatus.Running;
					Refresh();

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

			Logger.Info("End Process: XLIFF.Manager.Export" + Environment.NewLine + Environment.NewLine);
		}

		private bool Preparation(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				Logger.Info("Phase: Preparation");
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

				Logger.Error(ex.Message);
			}

			return success;
		}

		private bool Export(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				Logger.Info("Phase: Export");
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

				Logger.Error(ex.Message);
			}

			return success;
		}

		private bool Finalize(JobProcess jobProcess)
		{
			var success = true;

			try
			{
				Logger.Info("Phase: Finalize");

			
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

				Logger.Error(ex.Message);
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
