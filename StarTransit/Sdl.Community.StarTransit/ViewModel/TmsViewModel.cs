using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class TmsViewModel : WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _displayName;
		private string _tooltip;
		private string _errorMessage;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private bool? _checkAll;
		private bool _packageContainsTms;
		private readonly IWizardModel _wizardModel;
		private LanguagePair _selectedLanguagePair;
		private ICommand _selectTmCommand;
		private ICommand _removeTmCommand;
		private readonly IOpenFileDialogService _fileDialogService;

		public TmsViewModel(IWizardModel wizardModel,IOpenFileDialogService fileDialogService, object view) : base(view)
		{
			_currentPageNumber = 2;
			_displayName = PluginResources.Wizard_TM_DisplayName;
			_tooltip = PluginResources.Wizard_Tms_Tooltip;
			_wizardModel = wizardModel;
			IsPreviousEnabled = true;
			IsNextEnabled = true;
			_fileDialogService = fileDialogService;
			_checkAll = false;
			_isValid = true;
			PropertyChanged += TmsViewModelChanged;
		}

		public List<LanguagePair> LanguagePairsTmOptions
		{
			get => _wizardModel.PackageModel.Result?.LanguagePairs;
			set
			{
				_wizardModel.PackageModel.Result.LanguagePairs = value;
				OnPropertyChanged(nameof(LanguagePairsTmOptions));
			}
		}

		public LanguagePair SelectedLanguagePair
		{
			get => _selectedLanguagePair;
			set
			{
				_selectedLanguagePair = value;
				OnPropertyChanged(nameof(SelectedLanguagePair));
			}
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				if (_errorMessage == value) return;
				_errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
			}
		}

		public ICommand SelectTmCommand => _selectTmCommand ?? (_selectTmCommand = new RelayCommand(SelectTm));
		public ICommand RemoveSelectedTmCommand => _removeTmCommand ?? (_removeTmCommand = new RelayCommand(RemoveTm));

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

		public int CurrentPageNumber
		{
			get => _currentPageNumber;
			set
			{
				_currentPageNumber = value;
				OnPropertyChanged(nameof(CurrentPageNumber));
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
		public bool? CheckAll
		{
			get => _checkAll;
			set
			{
				if (_checkAll == value) return;
				_checkAll = value;
				CheckAllFiles(value);
				OnPropertyChanged(nameof(CheckAll));
			}
		}

		public bool PackageContainsTms
		{
			get => _wizardModel.PackageModel.Result.PackageContainsTms;
			set { _packageContainsTms = value; }
		}

		private void CheckAllFiles(bool? value)
		{
			if (value == null) return;
			foreach (var tmFile in LanguagePairsTmOptions.SelectMany(languagePairsTmOption => languagePairsTmOption.StarTranslationMemoryMetadatas))
			{
				tmFile.IsChecked = (bool)value;
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
				message = PluginResources.Wizard_ValidationMessage;
				return false;
			}
			return true;
		}

		private void TmsViewModelChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(CurrentPageChanged)) return;
			if (!IsCurrentPage) return;

			foreach (var languagePairsTmOption in LanguagePairsTmOptions)
			{
				languagePairsTmOption.SelectTmCommand = SelectTmCommand;
				languagePairsTmOption.RemoveSelectedTmCommand = RemoveSelectedTmCommand;
				languagePairsTmOption.TmOptionChangedEventRaised -= LanguagePairsTmOption_EventRaised;
				languagePairsTmOption.TmOptionChangedEventRaised += LanguagePairsTmOption_EventRaised;
				foreach (var tmMetadata in languagePairsTmOption.StarTranslationMemoryMetadatas)
				{
					tmMetadata.PropertyChanged -= TmMetadata_PropertyChanged;
					tmMetadata.PropertyChanged += TmMetadata_PropertyChanged;
				}
			}
		}

		private void TmMetadata_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!e.PropertyName.Equals(nameof(StarTranslationMemoryMetadata.IsChecked))) return;
			var allSelected = SelectedLanguagePair.StarTranslationMemoryMetadatas.All(t => t.IsChecked);
			if (allSelected)
			{
				CheckAll = true;
			}
			else
			{
				var allUnchecked = SelectedLanguagePair.StarTranslationMemoryMetadatas.All(t => !t.IsChecked);
				CheckAll = allUnchecked ? (bool?) false : null;
			}
		}

		private void LanguagePairsTmOption_EventRaised()
		{
			ErrorMessage = string.Empty;
		}

		private void RemoveTm()
		{
			ErrorMessage = string.Empty;
			SelectedLanguagePair.TmName = string.Empty;
			SelectedLanguagePair.TmPath = string.Empty;
		}

		private void SelectTm()
		{
			ErrorMessage = string.Empty;
			var selectedTm = _fileDialogService.ShowDialog("TM Files (.sdltm)|*.sdltm");
			if (SelectedLanguagePair is null || string.IsNullOrEmpty(selectedTm)) return;
			if (TmLanguageMatches(selectedTm))
			{
				SelectedLanguagePair.TmPath = selectedTm;
				SelectedLanguagePair.TmName = Path.GetFileNameWithoutExtension(selectedTm);
			}
			else
			{
				SelectedLanguagePair.TmPath = string.Empty;
				SelectedLanguagePair.TmName = string.Empty;
				ErrorMessage = PluginResources.Tm_LanguageValidation;
			}
		}

		private bool TmLanguageMatches(string selectedTmPath)
		{
			var tmInfo = new FileBasedTranslationMemory(selectedTmPath);
			var tmLanguageDirection = tmInfo.LanguageDirection;
			return SelectedLanguagePair.SourceLanguage.Name.Equals(tmLanguageDirection.SourceLanguage.Name) && SelectedLanguagePair.TargetLanguage.Name.Equals(tmLanguageDirection.TargetLanguage.Name);
		}
	}
}
