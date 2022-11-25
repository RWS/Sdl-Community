using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using GoogleTranslatorProvider.TellMe;
using GoogleTranslatorProvider.ViewModels;
using GoogleTranslatorProvider.Views;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace GoogleTranslatorProvider.Studio
{
	[TranslationProviderWinFormsUi(Id = "GoogleTranslatorProviderPlugin_WinFormsUI",
								   Name = "GoogleTranslatorProviderPlugin_WinFormsUI",
								   Description = "GoogleTranslatorProviderPlugin_WinFormsUI")]
	public class ProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		public string TypeDescription => PluginResources.Plugin_Description;

		public string TypeName => PluginResources.Plugin_NiceName;

		public bool SupportsEditing => true;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var options = new GTPTranslationOptions();
			var mainWindowViewModel = ShowProviderWindow(languagePairs, credentialStore, options);

			return mainWindowViewModel.DialogResult ? new ITranslationProvider[] { new Provider(options, new HtmlUtil()) }
													: null;
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is not Provider provider)
			{
				return false;
			}

			try
			{
				new SettingsAction().Execute();
				return true;
			}
			catch { }
			return false;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var options = new GTPTranslationOptions(translationProviderUri);
			if (options.SelectedProvider != ProviderType.GoogleTranslate)
			{
				return new TranslationProviderDisplayInfo
				{
					TranslationProviderIcon = PluginResources.my_icon,
					Name = PluginResources.Plugin_NiceName,
					TooltipText = PluginResources.Plugin_Tooltip
				};
			}

			var isV2 = options.SelectedGoogleVersion == ApiVersion.V2;
			var versionString = isV2 ? PluginResources.GoogleBasic : PluginResources.GoogleAdvanced;
			return new TranslationProviderDisplayInfo()
			{
				SearchResultImage = PluginResources.my_image,
				TranslationProviderIcon = PluginResources.my_icon,
				TooltipText = versionString,
				Name = versionString
			};
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Constants.GoogleTranslationScheme, StringComparison.CurrentCultureIgnoreCase);
		}

		private MainWindowViewModel ShowProviderWindow(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions)
		{
			SetSavedCredentialsOnUi(credentialStore, loadOptions);
			var providerControlViewModel = new ProviderViewModel(loadOptions);
			var settingsControlViewModel = new SettingsViewModel(loadOptions);
			var helpViewModel = new HelpViewModel();
			var mainWindowViewModel = new MainWindowViewModel(loadOptions, providerControlViewModel, settingsControlViewModel, helpViewModel, credentialStore, languagePairs, new HtmlUtil());
			var mainWindow = new MainWindowView { DataContext = mainWindowViewModel };
			mainWindowViewModel.CloseEventRaised += () =>
			{
				UpdateProviderCredentials(credentialStore, loadOptions);
				mainWindow.Close();
			};

			mainWindow.ShowDialog();
			return mainWindowViewModel;
		}

		private void UpdateProviderCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions options)
		{
			if (options.SelectedProvider == ProviderType.GoogleTranslate)
			{
				SetCredentialsOnCredentialStore(credentialStore, Constants.GoogleTranslationFullScheme, options.ApiKey, options.PersistGoogleKey);
			}
		}

		private void SetSavedCredentialsOnUi(ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions)
		{
			if (GetCredentialsFromStore(credentialStore, Constants.GoogleTranslationFullScheme) is TranslationProviderCredential googleCredentials)
			{
				loadOptions.ApiKey = googleCredentials.Credential;
				loadOptions.PersistGoogleKey = googleCredentials.Persist;
			}
		}

		private TranslationProviderCredential GetCredentialsFromStore(ITranslationProviderCredentialStore credentialStore, string uri)
		{
			var credentials = credentialStore.GetCredential(new Uri(uri));
			return credentials is not null ? new TranslationProviderCredential(credentials.Credential, credentials.Persist)
										   : null;
		}

		private void SetCredentialsOnCredentialStore(ITranslationProviderCredentialStore credentialStore, string providerUri, string apiKey, bool persistKey)
		{
			var uri = new Uri(providerUri);
			var credentials = new TranslationProviderCredential(apiKey, persistKey);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, credentials);
		}

		//TODO PACH (06/04/2021): Confirm if this is still required/ remove if obsolete code
		/// <summary>
		/// This gets called when a TranslationProviderAuthenticationException is thrown
		/// Since SDL Studio doesn't pass the provider instance here and even if we do a workaround...
		/// any new options set in the form that comes up are never saved to the project XML...
		/// so there is no way to change any options, only to provide the credentials
		/// </summary>
		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var projectInfo = projectController?.CurrentProject?.GetProjectInfo();
			var languagePairs = new List<LanguagePair>();
			if (projectInfo is not null)
			{
				foreach (var targetLanguage in projectInfo.TargetLanguages)
				{
					var languagePair = new LanguagePair(projectInfo.SourceLanguage.CultureInfo, targetLanguage.CultureInfo);
					languagePairs.Add(languagePair);
				}
			}

			var options = new GTPTranslationOptions();
			var mainWindowViewModel = ShowProviderWindow(languagePairs.ToArray(), credentialStore, options);
			return mainWindowViewModel.DialogResult;
		}
	}
}