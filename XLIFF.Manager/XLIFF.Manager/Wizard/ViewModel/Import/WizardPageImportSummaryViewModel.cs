using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportSummaryViewModel : WizardPageViewModelBase, IDisposable
	{
		public WizardPageImportSummaryViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			IsValid = true;
		}

		public override string DisplayName => PluginResources.PageName_Summary;
		public override bool IsValid { get; set; }
		public void Dispose()
		{
		}
	}
}
