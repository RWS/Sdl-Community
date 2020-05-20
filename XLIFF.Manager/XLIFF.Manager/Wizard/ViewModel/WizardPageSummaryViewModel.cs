using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageSummaryViewModel : WizardPageViewModelBase
	{
		private bool _isValid;

		public WizardPageSummaryViewModel(object view, TransactionModel transactionModel) : base(view, transactionModel)
		{
			IsValid = true;
		}

		public override string DisplayName => "Summary";

		public override bool IsValid
		{
			get => _isValid;
			set => _isValid = value;
		}
	}
}
