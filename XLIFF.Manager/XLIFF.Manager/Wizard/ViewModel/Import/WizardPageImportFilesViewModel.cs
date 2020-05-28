using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportFilesViewModel : WizardPageViewModelBase
	{
		public WizardPageImportFilesViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
		}

		public override string DisplayName => PluginResources.PageName_Files;
		public override bool IsValid { get; set; }
	}
}
