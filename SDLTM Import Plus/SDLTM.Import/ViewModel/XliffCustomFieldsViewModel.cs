using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using SDLTM.Import.Command;
using SDLTM.Import.Model;

namespace SDLTM.Import.ViewModel
{
    public class XliffCustomFieldsViewModel : WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _displayName;
		private string _xliffValueForFields;
		private string _tooltip;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private bool _shouldUseXliffValueForAllFields;
		private readonly WizardModel _wizardModel;
		private string _message;
		private ICommand _resetCommand;

		public XliffCustomFieldsViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_wizardModel = wizardModel;
			_currentPageNumber = 2;
			_isPreviousEnabled = true;
			_isNextEnabled = true;
			_displayName = PluginResources.Wizard_SecondPage_DisplayName;
			_tooltip = PluginResources.Wizard_SecondPage_Tooltip;
			_isValid = true;
			_message = string.Empty;
			PropertyChanged += XliffViewModelChanged;
		}

		public ObservableCollection<TmDetails> TmsList
		{
			get => _wizardModel?.TmsList;
			set
			{
				_wizardModel.TmsList = value;
				OnPropertyChanged(nameof(TmsList));
			}
		}

		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;

			var pagePosition = PageIndex - 1;
			if (position == pagePosition)
			{
				return false;
			}

			if (!IsValid && position > pagePosition)
			{
				message = PluginResources.CustomField_ValidationMessage;
				return false;
			}
			return true;
		}

		public int CurrentPageNumber
		{
			get => _currentPageNumber;
			set
			{
				_currentPageNumber = value;
				OnPropertyChanged(nameof(CurrentPageNumber));
			}
		}
		public override bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid == value)
					return;

				_isValid = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}
		public override string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName == value)
				{
					return;
				}

				_displayName = value;
				OnPropertyChanged(nameof(DisplayName));
			}
		}

		public override string Tooltip
		{
			get => _tooltip;
			set
			{
				if (_tooltip == value) return;
				_tooltip = value;
				OnPropertyChanged(Tooltip);
			}
		}

		public string XliffValueForFields
		{
			get => _xliffValueForFields;
			set
			{
				if (_xliffValueForFields == value) return;
				_xliffValueForFields = value;
				OnPropertyChanged(nameof(XliffValueForFields));
				CheckIfCustomXliffTuShouldBeUsed();
			}
		}

		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public bool IsNextEnabled
		{
			get => _isNextEnabled;
			set
			{
				if (_isNextEnabled == value)
					return;

				_isNextEnabled = value;
				OnPropertyChanged(nameof(IsNextEnabled));
			}
		}

		public bool ShouldUseXliffValueForAllFields
		{
			get => _shouldUseXliffValueForAllFields;
			set
			{
				if (_shouldUseXliffValueForAllFields == value) return;
				_shouldUseXliffValueForAllFields = value;
				OnPropertyChanged(nameof(ShouldUseXliffValueForAllFields));
				CheckIfCustomXliffTuShouldBeUsed();
			}
		}
		public bool IsPreviousEnabled
		{
			get => _isPreviousEnabled;
			set
			{
				if (_isPreviousEnabled == value)
					return;

				_isPreviousEnabled = value;
				OnPropertyChanged(nameof(IsPreviousEnabled));
			}
		}
		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(ClearSettings));

		private void ClearSettings(object obj)
		{
			ShouldUseXliffValueForAllFields = false;
			CheckIfCustomXliffTuShouldBeUsed();
			foreach (var tm in TmsList)
			{
				tm.SelectedSegmentField = null;
				tm.SegmentIdCustomFieldName = string.Empty;
				tm.CustomXliffFieldAlreadyExists = false;
			}
		}

		private void XliffViewModelChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(CurrentPageChanged)) return;
			if (!IsCurrentPage) return;
			foreach (var tm in TmsList)
			{
				tm.EditXliffFieldEventRaised += EditXliffFieldEventRaised;
			}
		}

		private void CheckIfCustomXliffTuShouldBeUsed()
		{
			if (!ShouldUseXliffValueForAllFields)	
			{
				XliffValueForFields = string.Empty;
			}
			SetTuNameOnTms(XliffValueForFields);
		}

		private void SetTuNameOnTms(string fieldValue)
		{
			foreach (var tm in TmsList)
			{
				tm.SegmentIdCustomFieldName = fieldValue;
			}
		}
		private void EditXliffFieldEventRaised(TmDetails tm)
		{
			if (tm.CustomXliffFieldAlreadyExists)
			{
				Message = $"Custom field {tm.SegmentIdCustomFieldName} already exists in this TM: {tm.Name}";
				IsValid = false;
			}
			if (IsWindowValid())
			{
				Message = string.Empty;
				IsValid = true;
			}
		}

		private bool IsWindowValid()
		{
			var valid = TmsList.Any(t => t.CustomXliffFieldAlreadyExists);
			return !valid;
		}
	}
}
