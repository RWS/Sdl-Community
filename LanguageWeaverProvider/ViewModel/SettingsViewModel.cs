using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		bool _autosendFeedback;
		bool _resendDrafts;
		bool _includeTags;
		bool _useCustomName;

		string _customName;

		public SettingsViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
			SetSettings();
		}

		public ITranslationOptions TranslationOptions { get; private set; }

		public bool AutosendFeedback
		{
			get => _autosendFeedback;
			set
			{
				_autosendFeedback = value;
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

		public bool IncludeTags
		{
			get => _includeTags;
			set
			{
				_includeTags = value;
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

		public ICommand BackCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

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
		}

		private void SetSettings()
		{
			TranslationOptions.ProviderSettings ??= new();
			AutosendFeedback = TranslationOptions.ProviderSettings.AutosendFeedback;
			ResendDrafts = TranslationOptions.ProviderSettings.ResendDrafts;
			IncludeTags = TranslationOptions.ProviderSettings.IncludeTags;
			UseCustomName = TranslationOptions.ProviderSettings.UseCustomName;
			CustomName = TranslationOptions.ProviderSettings.CustomName;
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
			}
		}
	}
}