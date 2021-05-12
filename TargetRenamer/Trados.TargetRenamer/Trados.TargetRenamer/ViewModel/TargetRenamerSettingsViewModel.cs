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
		private string _customLocation;
		private string _customString;
		private string _delimiter;
		private bool _overwriteTargetFiles;
		private string _regularExpressionReplaceWith;
		private string _regularExpressionSearchFor;
		private ICommand _resetToDefault;
		private string _selectedComboBoxItem;
		private ICommand _selectTargetFolder;
		private ICommand _clearCommand;
		private bool _useCustomLocation;
		private bool _useRegularExpression;
		private bool _useShortLocales;

		public TargetRenamerSettingsViewModel(IFolderDialogService folderDialogService)
		{
			_folderDialogService = folderDialogService;
			ComboBoxItems = new ObservableCollection<string> {AppendSuffixText, AppendPrefixText, RegExprText};
		}

		public ObservableCollection<string> ComboBoxItems { get; }

		public string SelectedComboBoxItem
		{
			get => _selectedComboBoxItem;
			set
			{
				if (_selectedComboBoxItem == value) return;
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

				_selectedComboBoxItem = value;
				OnPropertyChanged(nameof(SelectedComboBoxItem));
			}
		}

		public ICommand SelectTargetFolder =>
			_selectTargetFolder ?? (_selectTargetFolder = new CommandHandler(SelectFolder));

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

		public TargetRenamerSettings Settings { get; set; }

		private void SelectFolder(object obj)
		{
			CustomLocation = _folderDialogService.ShowDialog();
		}
		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(Clear));

		private void Clear(object obj)
		{
			Delimiter = string.Empty;
		}
	}
}