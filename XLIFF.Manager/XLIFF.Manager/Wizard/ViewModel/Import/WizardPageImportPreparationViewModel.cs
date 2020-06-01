using System;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportPreparationViewModel : WizardPageViewModelBase, IDisposable
	{
		public WizardPageImportPreparationViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			IsValid = true;
		}
		public override string DisplayName => PluginResources.PageName_Preparation;
		public override bool IsValid { get; set; }

		public void Dispose()
		{
		}
	}
}
