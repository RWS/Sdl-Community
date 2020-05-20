using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPagePreparationViewModel : WizardPageViewModelBase
	{
		private bool _isValid;
		private List<JobProcess> _jobProcesses;

		public WizardPagePreparationViewModel(object view, TransactionModel transactionModel) : base(view, transactionModel)
		{
			IsValid = true;
		}

		public List<JobProcess> JobProcesses
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
	}
}
