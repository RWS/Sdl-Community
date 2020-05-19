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
		private string _displayName;
		private bool _isValid;

		public WizardPageSummaryViewModel(object view, TransactionModel transactionModel) : base(view, transactionModel)
		{
			_displayName = "Summary";
			IsValid = true;
		}

		public override string DisplayName => _displayName;

		public override bool IsValid
		{
			get => _isValid;
			set => _isValid = value;
		}
	}
}
