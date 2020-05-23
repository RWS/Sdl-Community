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

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageExportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{		
		private bool _isValid;
		private List<XLIFFSupportModel> _xliffSupport;
		private XLIFFSupportModel _selectedXliffSupportModel;
		private string _outputFolder;
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;

		public WizardPageExportOptionsViewModel(Window owner, object view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
		{
			SelectedXliffSupport = XLIFFSupportList.FirstOrDefault(a => a.SupportType == WizardContext.XLIFFSupport);
			OutputFolder = WizardContext.OutputFolder;

			PropertyChanged += WizardPageExportOptionsViewModel_PropertyChanged;
		}
		
		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public List<XLIFFSupportModel> XLIFFSupportList
		{
			get
			{
				if (_xliffSupport != null)
				{
					return _xliffSupport;
				}

				_xliffSupport = new List<XLIFFSupportModel>
				{
					new XLIFFSupportModel
					{
						Name = "XLIFF 1.2 SDL",
						SupportType = Enumerators.XLIFFSupport.xliff12sdl
					},
					new XLIFFSupportModel
					{
						Name = "XLIFF 1.2 Polyglot",
						SupportType = Enumerators.XLIFFSupport.xliff12polyglot
					},
					new XLIFFSupportModel
					{
						Name = "XLIFF 2.0 SDL",
						SupportType = Enumerators.XLIFFSupport.xliff20sdl
					}
				};

				return _xliffSupport;
			}
		}

		public XLIFFSupportModel SelectedXliffSupport
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
			get { return _outputFolder; }
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
	
		public override string DisplayName => "Options";

		public override bool IsValid
		{
			get => _isValid;
			set => _isValid = value;
		}

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

		private void WizardPageExportOptionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					LoadChanges();
				}
				else
				{
					SaveChanges();
				}
			}
		}

		private void SaveChanges()
		{
			WizardContext.OutputFolder = OutputFolder;
			WizardContext.XLIFFSupport = SelectedXliffSupport.SupportType;
		}

		private void LoadChanges()
		{			
			VerifyIsValid();
		}

		public void Dispose()
		{
			PropertyChanged += WizardPageExportOptionsViewModel_PropertyChanged;
		}
	}
}
