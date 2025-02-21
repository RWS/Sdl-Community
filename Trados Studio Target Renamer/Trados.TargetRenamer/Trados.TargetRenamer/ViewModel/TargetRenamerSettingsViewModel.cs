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
	public class TargetRenamerSettingsViewModel : BaseViewModel, ISettingsAware<TargetRenamerSettings>
    {
        private const string AppendPrefixText = "Append As Prefix";
        private const string AppendSuffixText = "Append As Suffix";
        private const string RegExprText = "Use Regular Expression";
        private readonly IFolderDialogService _folderDialogService;
        private ICommand _clearCommand;
        private ICommand _resetToDefault;
        private string _selectedComboBoxItem;
        private ICommand _selectTargetFolder;
		private TargetRenamerSettings _settings;

		public TargetRenamerSettingsViewModel(IFolderDialogService folderDialogService)
		{
			_folderDialogService = folderDialogService;
			ComboBoxItems = new ObservableCollection<string> { AppendSuffixText, AppendPrefixText, RegExprText };
		}

        public bool AppendAsPrefix
        {
            get => Settings.AppendAsPrefix;
            set
            {
                if (Settings.AppendAsPrefix == value) return;
	            Settings.AppendAsPrefix = value;
                OnPropertyChanged(nameof(AppendAsPrefix));
            }
        }

		public bool AppendAsSuffix
		{
			get => Settings.AppendAsSuffix;
			set
			{
				if (Settings.AppendAsSuffix == value) return;
				Settings.AppendAsSuffix = value;
				OnPropertyChanged(nameof(AppendAsSuffix));
			}
		}

		public bool AppendCustomString
		{
			get => Settings.AppendCustomString;
			set
			{
				if (Settings.AppendCustomString == value) return;
				Settings.AppendCustomString = value;
				OnPropertyChanged(nameof(AppendCustomString));
			}
		}

		public bool AppendTargetLanguage
		{
			get => Settings.AppendTargetLanguage;
			set
			{
				if (Settings.AppendTargetLanguage == value) return;
				Settings.AppendTargetLanguage = value;
				OnPropertyChanged(nameof(AppendTargetLanguage));
			}
		}

        public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(Clear));
        public ObservableCollection<string> ComboBoxItems { get; }

		public string CustomLocation
		{
			get => Settings.CustomLocation;
			set
			{
				if (Settings.CustomLocation == value) return;
				Settings.CustomLocation = value;
				OnPropertyChanged(nameof(CustomLocation));
			}
		}

		public string CustomString
		{
			get => Settings.CustomString;
			set
			{
				if (Settings.CustomString == value) return;
				Settings.CustomString = value;
				OnPropertyChanged(nameof(CustomString));
			}
		}

		public string Delimiter
		{
			get => Settings.Delimiter;
			set
			{
				if (Settings.Delimiter == value) return;
				Settings.Delimiter = value;
				OnPropertyChanged(nameof(Delimiter));
			}
		}

		public string RegularExpressionReplaceWith
		{
			get => Settings.RegularExpressionReplaceWith;
			set
			{
				if (Settings.RegularExpressionReplaceWith == value) return;
				Settings.RegularExpressionReplaceWith = value;
				OnPropertyChanged(nameof(RegularExpressionReplaceWith));
			}
		}

		public string RegularExpressionSearchFor
		{
			get => Settings.RegularExpressionSearchFor;
			set
			{
				if (Settings.RegularExpressionSearchFor == value) return;
				Settings.RegularExpressionSearchFor = value;
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

		public TargetRenamerSettings Settings
		{
			get => _settings;
			set
			{
				if (string.IsNullOrWhiteSpace(value.CustomLocation)) value.CustomLocation = Path.GetTempPath();
				if (string.IsNullOrWhiteSpace(value.Delimiter)) value.Delimiter = "_";
				if (!value.AppendTargetLanguage) value.AppendTargetLanguage = true;
				_settings = value;

				SelectedComboBoxItem = AppendSuffixText;
			}
		}

		public bool UseCustomLocation
		{
			get => Settings.UseCustomLocation;
			set
			{
				if (Settings.UseCustomLocation == value) return;
				Settings.UseCustomLocation = value;
				OnPropertyChanged(nameof(UseCustomLocation));
			}
		}

		public bool UseRegularExpression
		{
			get => Settings.UseRegularExpression;
			set
			{
				if (Settings.UseRegularExpression == value) return;
				Settings.UseRegularExpression = value;
				OnPropertyChanged(nameof(UseRegularExpression));
			}
		}

		public bool UseShortLocales
		{
			get => Settings.UseShortLocales;
			set
			{
				if (Settings.UseShortLocales == value) return;
				Settings.UseShortLocales = value;
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