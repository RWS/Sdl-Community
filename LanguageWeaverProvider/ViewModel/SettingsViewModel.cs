using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using Microsoft.Win32;

namespace LanguageWeaverProvider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		bool _useCustomName;
		string _customName;

		bool _autosendFeedback;
		bool _resendDrafts;
		bool _includeTags;

		bool _usePreLookup;
		bool _usePostLookup;
		string _preLookupFilePath;
		string _postLookupFilePath;

		public SettingsViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
			SetSettings();
		}

		public ITranslationOptions TranslationOptions { get; private set; }

		public bool IncludeTags
		{
			get => _includeTags;
			set
			{
				_includeTags = value;
				OnPropertyChanged();
			}
		}

		public bool ResendDrafts
		{
			get => _resendDrafts;
			set
			{
				_resendDrafts = value;
				OnPropertyChanged();
			}
		}

		public bool AutosendFeedback
		{
			get => _autosendFeedback;
			set
			{
				_autosendFeedback = value;
				OnPropertyChanged();
			}
		}

		public bool UseCustomName
		{
			get => _useCustomName;
			set
			{
				_useCustomName = value;
				OnPropertyChanged();
			}
		}

		public string CustomName
		{
			get => _customName;
			set
			{
				_customName = value;
				OnPropertyChanged();
			}
		}

		public bool UsePreLookup
		{
			get => _usePreLookup;
			set
			{
				_usePreLookup = value;
				OnPropertyChanged();
			}
		}

		public bool UsePostLookup
		{
			get => _usePostLookup;
			set
			{
				_usePostLookup = value;
				OnPropertyChanged();
			}
		}

		public string PreLookupFilePath
		{
			get => _preLookupFilePath;
			set
			{
				_preLookupFilePath = value;
				OnPropertyChanged();
			}
		}

		public string PostLookupFilePath
		{
			get => _postLookupFilePath;
			set
			{
				_postLookupFilePath = value;
				OnPropertyChanged();
			}
		}

		public ICommand BackCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public ICommand BrowseFileCommand { get; private set; }

		public event EventHandler BackCommandExecuted;

		public bool SettingsAreValid()
		{
			return CustomNameIsValid();
		}

		private bool CustomNameIsValid()
		{
			if (!UseCustomName)
			{
				return true;
			}

			if (string.IsNullOrEmpty(CustomName))
			{
				ErrorHandling.ShowDialog(null, "Custom name", "The frienly provider name can not be empty if the option \"Friendly provider name\" is active.");
				return false;
			}

			CustomName = CustomName.Trim();
			var customNameIsSet = !string.IsNullOrEmpty(CustomName);
			if (!customNameIsSet)
			{
				ErrorHandling.ShowDialog(null, "Friendly name option", "The frienly provider name can not be empty if the option \"Friendly provider name\" is active.");
			}

			return customNameIsSet;
		}

		private void InitializeCommands()
		{
			BackCommand = new RelayCommand(Back);
			ClearCommand = new RelayCommand(Clear);
			BrowseFileCommand = new RelayCommand(BrowseFile);
		}

		private void SetSettings()
		{
			TranslationOptions.ProviderSettings ??= new();
			AutosendFeedback = TranslationOptions.ProviderSettings.AutosendFeedback;
			ResendDrafts = TranslationOptions.ProviderSettings.ResendDrafts;
			IncludeTags = TranslationOptions.ProviderSettings.IncludeTags;
			UseCustomName = TranslationOptions.ProviderSettings.UseCustomName;
			CustomName = TranslationOptions.ProviderSettings.CustomName;
			UsePreLookup = TranslationOptions.ProviderSettings.UsePrelookup;
			UsePostLookup = TranslationOptions.ProviderSettings.UsePostLookup;
			PreLookupFilePath = TranslationOptions.ProviderSettings.PreLookupFilePath;
			PostLookupFilePath = TranslationOptions.ProviderSettings.PostLookupFilePath;
		}

		private void Back(object parameter)
		{
			BackCommandExecuted?.Invoke(this, EventArgs.Empty);
		}

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case nameof(CustomName):
					CustomName = string.Empty;
					break;

				case nameof(PreLookupFilePath):
					PreLookupFilePath = string.Empty;
					break;

				case nameof(PostLookupFilePath):
					PostLookupFilePath = string.Empty;
					break;

				default:
					break;
			}
		}

		private void BrowseFile(object parameter)
		{
			if (parameter is not string target)
			{
				return;
			}

			var openFileDialog = new OpenFileDialog { Multiselect = false };
			var filePath = (bool)openFileDialog.ShowDialog() ? openFileDialog.FileName : string.Empty;
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			switch (target)
			{
				case nameof(PreLookupFilePath):
					PreLookupFilePath = filePath;
					ValidateLookupFile(PreLookupFilePath, target);
					break;

				case nameof(PostLookupFilePath):
					PostLookupFilePath = filePath;
					ValidateLookupFile(PostLookupFilePath, target);
					break;

				default:
					break;
			}
		}

		private void ValidateLookupFile(string filePath, string propertyName)
		{
			var lookup = new LWSegmentEditor(filePath);
			if (lookup.IsValid)
			{
				return;
			}

			string target;
			switch (propertyName)
			{
				case nameof(PreLookupFilePath):
					UsePreLookup = false;
					target = "Pre-Lookup Find/Replace";
					break;

				case nameof(PostLookupFilePath):
					UsePostLookup = false;
					target = "Post-Lookup Find/Replace";
					break;

				default:
					return;
			}

			Clear(propertyName);
			ErrorHandling.ShowDialog(null, "Oops! An error occurred", $"The chosen file is not in a valid format, and the {target} option has been disabled for now. Please see documentation for assistance.");
		}
	}
}