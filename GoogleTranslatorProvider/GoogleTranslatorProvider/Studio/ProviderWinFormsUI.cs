using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GoogleTranslatorProvider.Extensions;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
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

		public string TypeName => Constants.GoogleNaming_FullName;

		public bool SupportsEditing => true;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var options = new GTPTranslationOptions();
			var mainWindowViewModel = ShowRequestedView(languagePairs, credentialStore, options);
			return mainWindowViewModel.DialogResult ? new ITranslationProvider[] { new Provider(options) }
													: null;
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is not Provider editProvider)
			{
				return false;
			}

			var mainWindowViewModel = ShowRequestedView(languagePairs, credentialStore, editProvider.Options, true);
			return mainWindowViewModel.DialogResult;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var options = new GTPTranslationOptions(translationProviderUri);
			var customName = options.CustomProviderName;
			var useCustomName = options.UseCustomProviderName;
			var selectedVersion = options.SelectedGoogleVersion;
			var providerName = customName.SetProviderName(useCustomName, selectedVersion);
			return new TranslationProviderDisplayInfo()
			{
				SearchResultImage = PluginResources.my_image,
				TranslationProviderIcon = PluginResources.appicon,
				TooltipText = providerName,
				Name = providerName
			};
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return translationProviderUri switch
			{
				null => throw new ArgumentNullException(PluginResources.UriNotSupportedMessage),
				_ => string.Equals(translationProviderUri.Scheme, Constants.GoogleTranslationScheme, StringComparison.CurrentCultureIgnoreCase)
			};
		}

		private MainWindowViewModel ShowRequestedView(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions, bool showSettingsView = false)
		{
			SetSavedCredentialsOnUi(credentialStore, loadOptions);
			var mainWindowViewModel = new MainWindowViewModel(loadOptions, credentialStore, languagePairs, showSettingsView);
			var mainWindowView = new MainWindowView { DataContext = mainWindowViewModel };
			mainWindowViewModel.CloseEventRaised += () =>
			{
				UpdateProviderCredentials(credentialStore, loadOptions);
				mainWindowView.Close();
			};

			mainWindowView.ShowDialog();
			return mainWindowViewModel;
		}

		private void UpdateProviderCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions options)
		{
			if (options.SelectedProvider != ProviderType.GoogleTranslate)
			{
				return;
			}

			SetCredentialsOnCredentialStore(credentialStore, Constants.GoogleTranslationFullScheme, options.ApiKey, options.PersistGoogleKey);
		}

		private void SetSavedCredentialsOnUi(ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions)
		{
			var credentials = GetCredentialsFromStore(credentialStore, Constants.GoogleTranslationFullScheme);
			if (credentials is null)
			{
				return;
			}

			loadOptions.ApiKey = credentials.Credential;
			loadOptions.PersistGoogleKey = credentials.Persist;
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
			var mainWindowViewModel = ShowRequestedView(languagePairs.ToArray(), credentialStore, options);
			return mainWindowViewModel.DialogResult;
		}
	}
}