using LanguageMappingProvider.Database.Interface;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.LanguageMappingProvider;
using MicrosoftTranslatorProvider.LanguageMappingProvider.View;
using MicrosoftTranslatorProvider.LanguageMappingProvider.ViewModel;
using MicrosoftTranslatorProvider.View;
using Sdl.LanguagePlatform.Core;
using System;
using System.Linq;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Service;
using TradosProxySettings.View;
using TradosProxySettings.ViewModel;

namespace MicrosoftTranslatorProvider.ViewModel
{
    public class ProviderConfigurationViewModel : BaseViewModel
    {
		readonly ILanguageMappingDatabase _languageMappingDatabase;
		readonly ITranslationOptions _translationOptions;
		readonly LanguagePair[] _languagePairs;

		public ProviderConfigurationViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			_languagePairs = languagePairs;
			_translationOptions = translationOptions;
			_languageMappingDatabase = DatabaseControl.InitializeDatabase();
			AuthenticationType = _translationOptions.AuthenticationType;
			InitializeCommands();
			InitializeInternalView();
		}

		public bool SaveChanges { get; private set; }

		public AuthenticationType AuthenticationType { get; private set; }

        public MicrosoftConfigurationViewModel MicrosoftConfigurationViewModel { get; set; }

        public PrivateEndpointConfigurationViewModel PrivateEndpointConfigurationViewModel { get; set; }

        public ICommand ManageChangesCommand { get; private set; }
		public ICommand OpenLanguageMappingCommand { get; private set; }
		public ICommand OpenProviderSettingsCommand { get; private set; }
		public ICommand OpenProxySettingsCommand { get; private set; }

        public ICommand ResetAndIdentifyPairsCommand { get; private set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			ManageChangesCommand = new RelayCommand(ManageChanges);
			OpenLanguageMappingCommand = new RelayCommand(OpenLanguageMapping);
			OpenProviderSettingsCommand = new RelayCommand(OpenProviderSettings);
            OpenProxySettingsCommand = new RelayCommand(OpenProxySettings);
        }

		private void InitializeInternalView()
		{
			if (AuthenticationType == AuthenticationType.Microsoft)
			{
				MicrosoftConfigurationViewModel = new(_translationOptions, _languagePairs, _languageMappingDatabase);
			}

			if (AuthenticationType == AuthenticationType.PrivateEndpoint)
			{
				PrivateEndpointConfigurationViewModel = new(_translationOptions, _languagePairs, _languageMappingDatabase);
			}
		}

		private void OpenLanguageMapping(object parameter)
		{
			var lmpViewModel = new LanguageMappingProviderViewModel(_languageMappingDatabase);
			lmpViewModel.LanguageMappingUpdated += LanguageMappingUpdated;
			var lmpView = new LanguageMappingProviderView() { DataContext = lmpViewModel };
			lmpViewModel.CloseEventRaised += lmpView.Close;
			lmpView.ShowDialog();
		}

		private void LanguageMappingUpdated(object sender, EventArgs e)
		{
			MicrosoftConfigurationViewModel.CreatePairMappings();
		}

		private void OpenProviderSettings(object parameter)
		{
			var settingsViewModel = new SettingsViewModel(_translationOptions);
			var settingsView = new SettingsView() { DataContext = settingsViewModel };
			settingsViewModel.CloseEventRaised += settingsView.Close;
			settingsView.ShowDialog();
		}

        private void OpenProxySettings(object parameter)
        {
            var view = new ProxySettingsWindow();
            var viewModel = new ProxySettingsViewModel(view, _translationOptions.ProxySettings);
            view.DataContext = viewModel;
            view.ShowDialog();
            _translationOptions.ProxySettings = viewModel.ProxySettings;
            CredentialsManager.UpdateProxySettings(_translationOptions.ProxySettings);
        }

        private void ManageChanges(object parameter)
        {
            if (!bool.TryParse(parameter as string, out bool saveChanges))
            {
                return;
            }

            if (saveChanges)
            {
				_translationOptions.PairModels = AuthenticationType switch
				{
					AuthenticationType.Microsoft => [.. MicrosoftConfigurationViewModel.PairModels],
					AuthenticationType.PrivateEndpoint => [.. PrivateEndpointConfigurationViewModel.PairMappings]
				};

				if (AuthenticationType == AuthenticationType.PrivateEndpoint)
				{
					_translationOptions.PrivateEndpoint.Headers = [.. PrivateEndpointConfigurationViewModel.Headers];
					_translationOptions.PrivateEndpoint.Parameters = [.. PrivateEndpointConfigurationViewModel.Parameters];
				}
            }

            SaveChanges = saveChanges;
            CloseEventRaised.Invoke();
        }
    }
}