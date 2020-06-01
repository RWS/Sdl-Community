using System;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		public WizardPageImportOptionsViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			IsValid = true; //TODO remove this. Used to testing porpouse only
		}

		public override string DisplayName => PluginResources.PageName_Options;
		public override bool IsValid { get; set; }
		public void Dispose()
		{
		}
	}
}
