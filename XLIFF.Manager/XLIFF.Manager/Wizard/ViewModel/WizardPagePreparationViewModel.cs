using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPagePreparationViewModel : WizardPageViewModelBase
	{	
		private bool _isValid;
		private ObservableCollection<JobProcess> _jobProcesses;

		public WizardPagePreparationViewModel(Window owner, UserControl view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
		{	
			IsValid = true;
			InitializeJobProcessList();
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

		public override bool IsValid
		{
			get => _isValid;
			set => _isValid = value;
		}

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
					Name = "Export to XLIFF"
				},
				new JobProcess
				{
					Name = "Finalize"
				}
			};
		}
	}
}
