using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Trados.Transcreate.Commands;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Wizard.ViewModel.Convert
{
	public class WizardPageConvertOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		private readonly IDialogService _dialogService;
		private int _maxAlternativeTranslations;
		private bool _closeProjectOnComplete;
		private string _outputFolder;		
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;
		
		public WizardPageConvertOptionsViewModel(Window owner, object view, TaskContext taskContext, IDialogService dialogService) 
			: base(owner, view, taskContext)
		{
			_dialogService = dialogService;
			OutputFolder = TaskContext.WorkflowFolder;
			MaxAlternativeTranslations = taskContext.ConvertOptions.MaxAlternativeTranslations;
			CloseProjectOnComplete = taskContext.ConvertOptions.CloseProjectOnComplete;

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}
	
		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public string OutputFolder
		{
			get => _outputFolder;
			set
			{
				if (_outputFolder == value)
				{
					return;
				}

				_outputFolder = value;
				OnPropertyChanged(nameof(OutputFolder));

				VerifyIsValid();
			}
		}

		public int MaxAlternativeTranslations
		{
			get => _maxAlternativeTranslations;
			set
			{
				if (_maxAlternativeTranslations == value)
				{
					return;
				}

				_maxAlternativeTranslations = value;
				OnPropertyChanged(nameof(MaxAlternativeTranslations));
			}
		}

		public bool CloseProjectOnComplete
		{
			get => _closeProjectOnComplete;
			set
			{
				if (_closeProjectOnComplete == value)
				{
					return;
				}

				_closeProjectOnComplete = value;
				OnPropertyChanged(nameof(CloseProjectOnComplete));
			}
		}

		public override string DisplayName => PluginResources.PageName_Options;

		public override bool IsValid { get; set; }

		private void VerifyIsValid()
		{			
			IsValid = Directory.Exists(OutputFolder);
		}

		private void ClearExportFile(object parameter)
		{
			OutputFolder = string.Empty;
		}

		private void BrowseFolder(object parameter)
		{
			var folderPath = _dialogService.ShowFolderDialog(PluginResources.FolderDialog_Title, GetValidFolderPath());
			if (string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
			{
				OutputFolder = folderPath;
			}
		}


		private string GetValidFolderPath()
		{
			if (string.IsNullOrWhiteSpace(OutputFolder))
			{
				return string.Empty;
			}

			var outputFolder = OutputFolder;
			if (Directory.Exists(outputFolder))
			{
				return outputFolder;
			}

			while (outputFolder.Contains("\\"))
			{
				outputFolder = outputFolder.Substring(0, outputFolder.LastIndexOf("\\", StringComparison.Ordinal));
				if (Directory.Exists(outputFolder))
				{
					return outputFolder;
				}
			}

			return outputFolder;
		}

		private void OnLoadPage(object sender, EventArgs e)
		{			
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
			TaskContext.WorkflowFolder = OutputFolder;
			TaskContext.ConvertOptions.MaxAlternativeTranslations = MaxAlternativeTranslations;
			TaskContext.ConvertOptions.CloseProjectOnComplete = CloseProjectOnComplete;
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
