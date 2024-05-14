using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using static MicrosoftTranslatorProvider.ViewModel.ProviderConfigurationViewModel;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		readonly ITranslationOptions _translationOptions;

		public SettingsViewModel(ITranslationOptions translationOptions)
		{
			_translationOptions = translationOptions;
			ProviderSettings = _translationOptions.ProviderSettings.Clone();
			InitializeCommands();
		}

		public ProviderSettings ProviderSettings { get; private set; }

		public ICommand CloseCommand { get; private set; }
		public ICommand ClearCommand { get; private set; }
		public ICommand BrowseFileCommand { get; private set; }
		public ICommand ApplyChangesCommand { get; private set; }

		public event CloseWindowEventRaiser CloseEventRaised;

		public bool SettingsAreValid()
		{
			return CustomNameIsValid();
		}

		private bool CustomNameIsValid()
		{
			if (!ProviderSettings.UseCustomName)
			{
				return true;
			}

			if (string.IsNullOrEmpty(ProviderSettings.CustomName))
			{
				ErrorHandler.ShowDialog(null, "Custom name", "The frienly provider name can not be empty if the option \"Friendly provider name\" is active.");
				return false;
			}

			ProviderSettings.CustomName = ProviderSettings.CustomName.Trim();
			var customNameIsSet = !string.IsNullOrEmpty(ProviderSettings.CustomName);
			if (!customNameIsSet)
			{
				ErrorHandler.ShowDialog(null, "Friendly name option", "The frienly provider name can not be empty if the option \"Friendly provider name\" is active.");
			}

			return customNameIsSet;
		}

		private void InitializeCommands()
		{
			CloseCommand = new RelayCommand(Close);
			ClearCommand = new RelayCommand(Clear);
			BrowseFileCommand = new RelayCommand(BrowseFile);
			ApplyChangesCommand = new RelayCommand(ApplyChanges, ChangesHasBeenAplied);
		}

		private bool ChangesHasBeenAplied(object parameter)
		{
			var currentSettings = _translationOptions.ProviderSettings;
			var properties = currentSettings.GetType().GetProperties();
			return properties.Any(property => !Equals(property.GetValue(currentSettings), property.GetValue(ProviderSettings)));
		}

		private void ApplyChanges(object parameter)
		{
			if (SettingsAreValid())
			{
				_translationOptions.ProviderSettings = ProviderSettings.Clone();
			}
		}

		private void Close(object parameter)
		{
			CloseEventRaised.Invoke();
		}

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case nameof(ProviderSettings.CustomName):
					ProviderSettings.CustomName = string.Empty;
					break;

				case nameof(ProviderSettings.PreLookupFilePath):
					ProviderSettings.PreLookupFilePath = string.Empty;
					break;

				case nameof(ProviderSettings.PostLookupFilePath):
                    ProviderSettings.PostLookupFilePath = string.Empty;
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
				case nameof(ProviderSettings.PreLookupFilePath):
					ProviderSettings.PreLookupFilePath = filePath;
					ValidateLookupFile(ProviderSettings.PreLookupFilePath, target);
					break;

				case nameof(ProviderSettings.PostLookupFilePath):
					ProviderSettings.PostLookupFilePath = filePath;
					ValidateLookupFile(ProviderSettings.PostLookupFilePath, target);
					break;

				default:
					break;
			}
		}

		private void ValidateLookupFile(string filePath, string propertyName)
		{
			var lookup = new MicrosoftSegmentEditor(filePath);
			if (lookup.IsValid)
			{
				return;
			}

			string target;
			switch (propertyName)
			{
				case nameof(ProviderSettings.PreLookupFilePath):
					ProviderSettings.UsePreLookup = false;
					target = "Pre-Lookup Find/Replace";
					break;

				case nameof(ProviderSettings.PostLookupFilePath):
					ProviderSettings.UsePostLookup = false;
					target = "Post-Lookup Find/Replace";
					break;

				default:
					return;
			}

			Clear(propertyName);
			ErrorHandler.ShowDialog(null, "Oops! An error occurred", $"The chosen file is not in a valid format, and the {target} option has been disabled for now. Please see documentation for assistance.");
		}
	}
}