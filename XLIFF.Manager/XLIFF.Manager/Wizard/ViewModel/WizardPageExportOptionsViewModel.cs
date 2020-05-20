using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageExportOptionsViewModel : WizardPageViewModelBase
	{
		private bool _isValid;
		private List<XLIFFSupportModel> _xliffSupport;
		private XLIFFSupportModel _selectedXliffSupportModel;
		private string _exportFile;
		private ICommand _clearExportFileCommand;

		public WizardPageExportOptionsViewModel(object view, TransactionModel transactionModel) : base(view, transactionModel)
		{			
			IsValid = true;

			SelectedXliffSupportModel =
				XLIFFSupport.FirstOrDefault(a => a.SupportType == Enumerators.XLIFFSupport.xliff12polyglot);

			ExportFile = string.Empty;

			PropertyChanged += WizardPageOptionsViewModel_PropertyChanged;
		}

		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

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

		public string ExportFile
		{
			get { return _exportFile; }
			set
			{
				if (_exportFile == value)
				{
					return;
				}

				_exportFile = value;
				OnPropertyChanged(nameof(ExportFile));
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

		private void ClearExportFile(object parameter)
		{
			ExportFile = string.Empty;
		}
	}
}
