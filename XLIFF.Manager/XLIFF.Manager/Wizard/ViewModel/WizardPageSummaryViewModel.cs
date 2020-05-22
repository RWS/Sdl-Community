using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageSummaryViewModel : WizardPageViewModelBase
	{	
		private bool _isValid;

		public WizardPageSummaryViewModel(Window owner, object view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
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
