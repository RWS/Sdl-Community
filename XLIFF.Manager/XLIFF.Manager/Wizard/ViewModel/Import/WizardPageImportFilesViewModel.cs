using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportFilesViewModel : WizardPageViewModelBase,IDisposable
	{
		private ICommand _addFolderCommand;
		private ICommand _addFilesCommand;
		private readonly IDialogService _dialogService;
		private List<ProjectFile> _projectFiles;


		public WizardPageImportFilesViewModel(Window owner, object view, WizardContext wizardContext,IDialogService dialogService) : base(owner, view, wizardContext)
		{
			_dialogService = dialogService;
			ProjectFiles = wizardContext.ProjectFiles;

			IsValid = true; //TODO remove this. Used to testing porpouse only
		}

		public ICommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new RelayCommand(SelectFolder));
		public ICommand AddFilesCommand => _addFilesCommand ?? (_addFilesCommand = new RelayCommand(AddFiles));

		public List<ProjectFile> ProjectFiles
		{
			get => _projectFiles ?? (_projectFiles = new List<ProjectFile>());
			set => _projectFiles = value;
		}

		private void AddFiles()
		{
			var selectedFiles = _dialogService.ShowFileDialog("Xliff (*.xliff) |*.xliff", PluginResources.FilesDialog_Title);
		}

		private void SelectFolder()
		{
			var folderPath = _dialogService.ShowFolderDialog(PluginResources.FolderDialog_Title);
		}

		public override string DisplayName => PluginResources.PageName_Files;
		public override bool IsValid { get; set; }

		public void Dispose()
		{
		}
	}
}
