using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using UserControl = System.Windows.Controls.UserControl;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageExportOptionsViewModel : WizardPageViewModelBase
	{		
		private bool _isValid;
		private List<XLIFFSupportModel> _xliffSupport;
		private XLIFFSupportModel _selectedXliffSupportModel;
		private string _outputFolder;
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;

		public WizardPageExportOptionsViewModel(Window owner, UserControl view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
		{		
			IsValid = true;

			SelectedXliffSupportModel =
				XLIFFSupport.FirstOrDefault(a => a.SupportType == Enumerators.XLIFFSupport.xliff12polyglot);

			OutputFolder = string.Empty;

			PropertyChanged += WizardPageOptionsViewModel_PropertyChanged;
		}

		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public List<XLIFFSupportModel> XLIFFSupport
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

		public XLIFFSupportModel SelectedXliffSupportModel
		{
			get
			{
				return _selectedXliffSupportModel
					   ?? (_selectedXliffSupportModel = XLIFFSupport.FirstOrDefault(a => a.SupportType == Enumerators.XLIFFSupport.xliff12polyglot));
			}
			set
			{
				if (_selectedXliffSupportModel == value)
				{
					return;
				}

				_selectedXliffSupportModel = value;
				OnPropertyChanged(nameof(SelectedXliffSupportModel));
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

		private void WizardPageOptionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{

				}
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
	}
}
