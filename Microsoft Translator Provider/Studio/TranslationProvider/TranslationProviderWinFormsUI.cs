﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.View;
using MicrosoftTranslatorProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace MicrosoftTranslatorProvider.Studio
{
	[TranslationProviderWinFormsUi(Id = "MicrosoftTranslatorProviderPlugin_WinFormsUI",
								   Name = "MicrosoftTranslatorProviderPlugin_WinFormsUI",
                                   Description = "MicrosoftTranslatorProviderPlugin_WinFormsUI")]
    public class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
		public string TypeDescription => PluginResources.Plugin_Description;
		public string TypeName => PluginResources.Plugin_NiceName;
		public bool SupportsEditing => true;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			ApplicationInitializer.CredentialStore = credentialStore;

			var translationOptions = new TranslationOptions(true);
			CredentialsManager.GetCredentials(translationOptions);
            CredentialsManager.GetProxySettings(translationOptions);

            var authenticationViewModel = new AuthenticationViewModel(translationOptions);
			var authenticationView = new AuthenticationView()
			{
				DataContext = authenticationViewModel,
			};
			authenticationViewModel.CloseEventRaised += () =>
			{
				translationOptions.UpdateUri();
				CredentialsManager.UpdateCredentials(translationOptions);
				CredentialsManager.UpdateProxySettings(translationOptions);
				authenticationView.Close();
			};

			authenticationView.ShowDialog();
			if (!authenticationViewModel.SaveChanges)
			{
				return null;
			}

			var providerConfigurationViewModel = new ProviderConfigurationViewModel(translationOptions, languagePairs);
			var providerConfigurationView = new ProviderConfigurationView
			{
				DataContext = providerConfigurationViewModel,
			};
			providerConfigurationViewModel.CloseEventRaised += providerConfigurationView.Close;

			providerConfigurationView.ShowDialog();
			if (!providerConfigurationViewModel.SaveChanges)
			{
				return null;
			}

            var provider = new TranslationProvider(translationOptions);
            SaveEditChanges(provider);
            
            return [provider];
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			ApplicationInitializer.CredentialStore = credentialStore;
			if (translationProvider is not TranslationProvider editProvider)
			{
				return false;
			}

			var providerConfigurationViewModel = new ProviderConfigurationViewModel(editProvider.TranslationOptions, languagePairs);
			var providerConfigurationView = new ProviderConfigurationView { DataContext = providerConfigurationViewModel };
			providerConfigurationViewModel.CloseEventRaised += providerConfigurationView.Close;
			providerConfigurationView.ShowDialog();

            if (providerConfigurationViewModel.SaveChanges)
            {
                SaveEditChanges(editProvider);
            }

            return providerConfigurationViewModel.SaveChanges;
		}

        private static void SaveEditChanges(TranslationProvider editProvider)
        {
            var translationOptions = editProvider.TranslationOptions;
            CredentialsManager.UpdateProxySettings(translationOptions);
            CredentialsManager.UpdateCredentials(translationOptions);
            ApplicationInitializer.TranslationOptions[translationOptions.Id] = translationOptions;
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			if (translationProviderState is null)
			{
				return new TranslationProviderDisplayInfo()
				{
					SearchResultImage = PluginResources.microsoft_image,
					TranslationProviderIcon = PluginResources.mstp_icon,
					TooltipText = PluginResources.Microsoft_NiceName,
					Name = PluginResources.Microsoft_NiceName
				};
			}

			var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			var providerName = options.ProviderName;
			return new TranslationProviderDisplayInfo()
			{
				SearchResultImage = PluginResources.microsoft_image,
				TranslationProviderIcon = PluginResources.mstp_icon,
				TooltipText = providerName,
				Name = providerName
			};
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderScheme, StringComparison.OrdinalIgnoreCase)
				|| string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderPrivateEndpointScheme, StringComparison.OrdinalIgnoreCase);
		}

		//TODO PACH (06/04/2021): Confirm if this is still required/ remove if obsolete code
		/// <summary>
		/// This gets called when a TranslationProviderAuthenticationException is thrown
		/// Since SDL Studio doesn't pass the provider instance here and even if we do a workaround...
		/// any new options set in the form that comes up are never saved to the project XML...
		/// so there is no way to change any options, only to provide the credentials
		/// </summary>
		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			var languagePairs = new List<LanguagePair>();
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			if (projectController?.CurrentProject?.GetProjectInfo() is ProjectInfo projectInfo)
			{
				foreach (var targetLanguage in projectInfo.TargetLanguages)
				{
					var languagePair = new LanguagePair(projectInfo.SourceLanguage.CultureInfo, targetLanguage.CultureInfo);
					languagePairs.Add(languagePair);
				}
			}

			var options = new TranslationOptions();
			//var mainWindowViewModel = ShowProviderWindow(languagePairs.ToArray(), credentialStore, options);
			//return mainWindowViewModel.DialogResult;
			return false;
		}
	}
}