using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Export
{
	public class WizardPageExportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{		
		private List<XLIFFSupport> _xliffSupport;
		private XLIFFSupport _selectedXliffSupportModel;
		private string _outputFolder;
		private bool _copySourceToTarget;
		private bool _copySourceToTargetEnabled;
		private bool _includeTranslations;
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;

		public WizardPageExportOptionsViewModel(Window owner, object view, WizardContext wizardContext) 
			: base(owner, view, wizardContext)
		{		
			SelectedXliffSupport = XLIFFSupportList.FirstOrDefault(a => a.SupportType == WizardContext.Support);
			OutputFolder = WizardContext.OutputFolder;
			CopySourceToTarget = wizardContext.CopySourceToTarget;
			IncludeTranslations = wizardContext.IncludeTranslations;

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}
		
		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public List<XLIFFSupport> XLIFFSupportList
		{
			get
			{
				if (_xliffSupport != null)
				{
					return _xliffSupport;
				}

				_xliffSupport = new List<XLIFFSupport>
				{
					new XLIFFSupport
					{
						Name = "XLIFF 1.2 SDL",
						SupportType = Enumerators.XLIFFSupport.xliff12sdl
					},
					new XLIFFSupport
					{
						Name = "XLIFF 1.2 Polyglot",
						SupportType = Enumerators.XLIFFSupport.xliff12polyglot
					}
					// TODO spport for this format will come later on in the development cycle
					//new XLIFFSupportModel
					//{
					//	Name = "XLIFF 2.0 SDL",
					//	SupportType = Enumerators.XLIFFSupport.xliff20sdl
					//}
				};

				return _xliffSupport;
			}
		}

		public XLIFFSupport SelectedXliffSupport
		{
			get
			{
				return _selectedXliffSupportModel
					   ?? (_selectedXliffSupportModel = XLIFFSupportList.FirstOrDefault(a => a.SupportType == Enumerators.XLIFFSupport.xliff12polyglot));
			}
			set
			{
				if (_selectedXliffSupportModel == value)
				{
					return;
				}

				_selectedXliffSupportModel = value;
				OnPropertyChanged(nameof(SelectedXliffSupport));
			}
		}

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

		public bool CopySourceToTarget
		{
			get => _copySourceToTarget;
			set
			{
				if (_copySourceToTarget == value)
				{
					return;
				}

				_copySourceToTarget = value;
				OnPropertyChanged(nameof(CopySourceToTarget));

				VerifyIsValid();
			}
		}

		public bool CopySourceToTargetEnabled
		{
			get => _copySourceToTargetEnabled;
			set
			{
				if (_copySourceToTargetEnabled == value)
				{
					return;
				}

				_copySourceToTargetEnabled = value;
				OnPropertyChanged(nameof(CopySourceToTargetEnabled));			
			}
		}

		public bool IncludeTranslations
		{
			get => _includeTranslations;
			set
			{
				if (_includeTranslations == value)
				{
					return;
				}

				_includeTranslations = value;
				OnPropertyChanged(nameof(IncludeTranslations));

				if (!_includeTranslations)
				{
					CopySourceToTarget = false;
					CopySourceToTargetEnabled = false;
				}
				else
				{
					CopySourceToTargetEnabled = true;
				}

				VerifyIsValid();
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
			var browser = new FolderBrowserDialog();
			browser.SelectedPath = GetValidFolderPath();
			browser.Description = "Select the output folder";

			if (browser.ShowDialog() == DialogResult.OK)
			{
				OutputFolder = browser.SelectedPath;
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
			WizardContext.OutputFolder = OutputFolder;
			WizardContext.Support = SelectedXliffSupport.SupportType;
			WizardContext.CopySourceToTarget = CopySourceToTarget;
			WizardContext.IncludeTranslations = IncludeTranslations;
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
