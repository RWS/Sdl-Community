using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportFilesViewModel : WizardPageViewModelBase,IDisposable
	{
		private ICommand _addFolderCommand;
		private ICommand _addFilesCommand;


		public WizardPageImportFilesViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			IsValid = true; //TODO remove this. Used to testing porpouse only
		}

		public ICommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new RelayCommand(SelectFolder));
		public ICommand AddFilesCommand => _addFilesCommand ?? (_addFilesCommand = new RelayCommand(AddFiles));

		private void AddFiles()
		{
			
		}


		private void SelectFolder()
		{
			
		}

		public override string DisplayName => PluginResources.PageName_Files;
		public override bool IsValid { get; set; }

		public void Dispose()
		{
		}
	}
}
