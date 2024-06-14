using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using SDLTM.Import.Command;
using SDLTM.Import.Model;

namespace SDLTM.Import.ViewModel
{
	public class FileNameCustomFieldViewModel : WizardViewModelBase
	{
		private int _currentPageNumber;
		private readonly WizardModel _wizardModel;
		private string _displayName;
		private string _fileNameValueForFields;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private bool _shouldUseFileNameValueForAllFields;
		private string _message;
		private string _tooltip;
		private ICommand _resetCommand;

		public FileNameCustomFieldViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_wizardModel = wizardModel;
			_currentPageNumber = 3;
			_isPreviousEnabled = true;
			_isNextEnabled = true;
			_displayName = PluginResources.Wizard_ThirdPage_DisplayName;
			_tooltip = PluginResources.Wizard_ThirdPageTooltip;
			_isValid = true;
			_message = string.Empty;
			PropertyChanged += FileNameViewModelChanged;
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

		public ObservableCollection<TmDetails> TmsList
		{
			get => _wizardModel?.TmsList;
			set
			{
				_wizardModel.TmsList = value;
				OnPropertyChanged(nameof(TmsList));
			}
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

		public string FileNameValueForFields
		{
			get => _fileNameValueForFields;
			set
			{
				if (_fileNameValueForFields == value) return;
				_fileNameValueForFields = value;
				OnPropertyChanged(nameof(FileNameValueForFields));
				CheckIfCustomFileNameTuShouldBeUsed();
			}
		}
		public bool ShouldUseFileNameValueForAllFields
		{
			get => _shouldUseFileNameValueForAllFields;
			set
			{
				if (_shouldUseFileNameValueForAllFields == value) return;
				_shouldUseFileNameValueForAllFields = value;
				OnPropertyChanged(nameof(ShouldUseFileNameValueForAllFields));
				CheckIfCustomFileNameTuShouldBeUsed();
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
		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}
		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(ClearSettings));

		private void ClearSettings(object obj)
		{
			ShouldUseFileNameValueForAllFields = false;
			CheckIfCustomFileNameTuShouldBeUsed();
			foreach (var tm in TmsList)
			{
				tm.SelectedFileNameField = null;
				tm.FileNameCustomField = string.Empty;
				tm.CustomFileNameFieldAlreadyExists = false;
			}
		}

		private void CheckIfCustomFileNameTuShouldBeUsed()
		{
			if (!ShouldUseFileNameValueForAllFields)
			{
				FileNameValueForFields = string.Empty;
			}
			SetTuNameOnTms(FileNameValueForFields);
		}

		private void SetTuNameOnTms(string fieldValue)
		{
			foreach (var tm in TmsList)
			{
				tm.FileNameCustomField = fieldValue;
			}
		}
		private void FileNameViewModelChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(CurrentPageChanged)) return;
			if (!IsCurrentPage) return;
			foreach (var tm in TmsList)
			{
				tm.EditFileNameFieldEventRaised += EditFileNameFieldEventRaised;	
			}
		}

		private void EditFileNameFieldEventRaised(TmDetails tm)
		{
			if (tm.CustomFileNameFieldAlreadyExists)
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
			var valid = TmsList.Any(t => t.CustomFileNameFieldAlreadyExists);
			return !valid;
		}
	}
}
