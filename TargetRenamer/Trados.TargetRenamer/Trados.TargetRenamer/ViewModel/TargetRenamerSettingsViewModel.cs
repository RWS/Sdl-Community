using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Sdl.Desktop.IntegrationApi;
using Trados.TargetRenamer.BatchTask;
using Trados.TargetRenamer.Interfaces;
using Trados.TargetRenamer.Model;
using Trados.TargetRenamer.Services;

namespace Trados.TargetRenamer.ViewModel
{
	public class TargetRenamerSettingsViewModel : BaseModel, ISettingsAware<TargetRenamerSettings>
	{
		private const string AppendPrefixText = "Append As Prefix";
		private const string AppendSuffixText = "Append As Suffix";
		private const string RegExprText = "Use Regular Expression";
		private readonly IFolderDialogService _folderDialogService;
		private bool _appendAsPrefix;
		private bool _appendAsSuffix;
		private bool _appendCustomString;
		private bool _appendTargetLanguage;
		private ICommand _clearCommand;
		private string _customLocation;
		private string _customString;
		private string _delimiter;
		private bool _overwriteTargetFiles;
		private string _regularExpressionReplaceWith;
		private string _regularExpressionSearchFor;
		private ICommand _resetToDefault;
		private string _selectedComboBoxItem;
		private ICommand _selectTargetFolder;
		private bool _useCustomLocation;
		private bool _useRegularExpression;
		private bool _useShortLocales;

		public TargetRenamerSettingsViewModel(IFolderDialogService folderDialogService)
		{
			_folderDialogService = folderDialogService;
			ComboBoxItems = new ObservableCollection<string> { AppendSuffixText, AppendPrefixText, RegExprText };
			Reset(null);
		}

		public bool AppendAsPrefix
		{
			get => _appendAsPrefix;
			set
			{
				if (_appendAsPrefix == value) return;
				_appendAsPrefix = value;
				OnPropertyChanged(nameof(AppendAsPrefix));
			}
		}

		public bool AppendAsSuffix
		{
			get => _appendAsSuffix;
			set
			{
				if (_appendAsSuffix == value) return;
				_appendAsSuffix = value;
				OnPropertyChanged(nameof(AppendAsSuffix));
			}
		}

		public bool AppendCustomString
		{
			get => _appendCustomString;
			set
			{
				if (_appendCustomString == value) return;
				_appendCustomString = value;
				OnPropertyChanged(nameof(AppendCustomString));
			}
		}

		public bool AppendTargetLanguage
		{
			get => _appendTargetLanguage;
			set
			{
				if (_appendTargetLanguage == value) return;
				_appendTargetLanguage = value;
				OnPropertyChanged(nameof(AppendTargetLanguage));
			}
		}

		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(Clear));
		public ObservableCollection<string> ComboBoxItems { get; }

		public string CustomLocation
		{
			get => _customLocation;
			set
			{
				if (_customLocation == value) return;
				_customLocation = value;
				OnPropertyChanged(nameof(CustomLocation));
			}
		}

		public string CustomString
		{
			get => _customString;
			set
			{
				if (_customString == value) return;
				_customString = value;
				OnPropertyChanged(nameof(CustomString));
			}
		}

		public string Delimiter
		{
			get => _delimiter;
			set
			{
				if (string.IsNullOrWhiteSpace(_delimiter)) _delimiter = "_";
				if (_delimiter == value) return;
				_delimiter = value;
				OnPropertyChanged(nameof(Delimiter));
			}
		}

		public bool HasErrors { get; internal set; } = false;

		public bool OverwriteTargetFiles
		{
			get => _overwriteTargetFiles;
			set
			{
				if (_overwriteTargetFiles == value) return;
				_overwriteTargetFiles = value;
				OnPropertyChanged(nameof(OverwriteTargetFiles));
			}
		}

		public string RegularExpressionReplaceWith
		{
			get => _regularExpressionReplaceWith;
			set
			{
				if (_regularExpressionReplaceWith == value) return;
				_regularExpressionReplaceWith = value;
				OnPropertyChanged(nameof(RegularExpressionReplaceWith));
			}
		}

		public string RegularExpressionSearchFor
		{
			get => _regularExpressionSearchFor;
			set
			{
				if (_regularExpressionSearchFor == value) return;
				_regularExpressionSearchFor = value;
				OnPropertyChanged(nameof(RegularExpressionSearchFor));
			}
		}

		public ICommand ResetToDefault => _resetToDefault ?? (_resetToDefault = new CommandHandler(Reset));

		public string SelectedComboBoxItem
		{
			get => _selectedComboBoxItem;
			set
			{
				if (_selectedComboBoxItem == value) return;
				OnSelectedComboBoxValueChanged(value);

				_selectedComboBoxItem = value;
				OnPropertyChanged(nameof(SelectedComboBoxItem));
			}
		}

		public ICommand SelectTargetFolder =>
			_selectTargetFolder ?? (_selectTargetFolder = new CommandHandler(SelectFolder));

		public TargetRenamerSettings Settings { get; set; }

		public bool UseCustomLocation
		{
			get => _useCustomLocation;
			set
			{
				if (_useCustomLocation == value) return;
				_useCustomLocation = value;
				OnPropertyChanged(nameof(UseCustomLocation));
			}
		}

		public bool UseRegularExpression
		{
			get => _useRegularExpression;
			set
			{
				if (_useRegularExpression == value) return;
				_useRegularExpression = value;
				OnPropertyChanged(nameof(UseRegularExpression));
			}
		}

		public bool UseShortLocales
		{
			get => _useShortLocales;
			set
			{
				if (_useShortLocales == value) return;
				_useShortLocales = value;
				OnPropertyChanged(nameof(UseShortLocales));
			}
		}

		private void Clear(object obj)
		{
			Delimiter = string.Empty;
		}

		private void OnSelectedComboBoxValueChanged(string value)
		{
			switch (value)
			{
				case AppendSuffixText:
					AppendAsSuffix = true;
					AppendAsPrefix = false;
					UseRegularExpression = false;
					break;

				case AppendPrefixText:
					AppendAsPrefix = true;
					AppendAsSuffix = false;
					UseRegularExpression = false;
					break;

				case RegExprText:
					UseRegularExpression = true;
					AppendAsPrefix = false;
					AppendAsSuffix = false;
					AppendCustomString = false;
					AppendTargetLanguage = false;
					break;
			}
		}

		private void Reset(object obj)
		{
			OverwriteTargetFiles = true;
			AppendAsPrefix = false;
			AppendAsSuffix = true;
			UseCustomLocation = false;
			CustomLocation = Path.GetTempPath();
			RegularExpressionSearchFor = string.Empty;
			RegularExpressionReplaceWith = string.Empty;
			Delimiter = "_";
			UseShortLocales = false;
			AppendTargetLanguage = true;
			AppendCustomString = false;
			CustomString = string.Empty;
			SelectedComboBoxItem = AppendSuffixText;
		}

		private void SelectFolder(object obj)
		{
			CustomLocation = _folderDialogService.ShowDialog();
		}
	}
}