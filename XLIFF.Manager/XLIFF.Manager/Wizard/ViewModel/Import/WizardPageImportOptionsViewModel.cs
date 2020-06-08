using System;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		public WizardPageImportOptionsViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			VerifyIsValid();
		}

		public override string DisplayName => PluginResources.PageName_Options;

		public override bool IsValid { get; set; }

		private void VerifyIsValid()
		{
			IsValid = true;
		}


		public void Dispose()
		{
		}
	}
}
