using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NLog;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.ViewModels;
using GoogleTranslatorProvider.Views;
using LogManager = NLog.LogManager;

namespace GoogleTranslatorProvider.Studio
{
	[TranslationProviderWinFormsUi(Id = "GoogleTranslatorProviderPlugin_WinFormsUI",
								   Name = "GoogleTranslatorProviderPlugin_WinFormsUI",
								   Description = "GoogleTranslatorProviderPlugin_WinFormsUI")]
	public class ProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		public string TypeDescription => PluginResources.Plugin_Description;
		public string TypeName => PluginResources.Plugin_NiceName;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			var options = new GTPTranslationOptions();
			var htmlUtil = new HtmlUtil();

			var mainWindowVm = ShowProviderWindow(languagePairs, credentialStore, options);

			if (!mainWindowVm.DialogResult) return null;

			var provider = new Provider(options, htmlUtil);

			return new ITranslationProvider[] { provider };
		}

		/// <summary>
		/// Determines whether the plug-in settings can be changed
		/// by displaying the Settings button in SDL Trados Studio.
		/// </summary>
		public bool SupportsEditing => true;

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			if (!(translationProvider is Provider editProvider))
			{
				return false;
			}

			var mainWindowVm = ShowProviderWindow(languagePairs, credentialStore, editProvider.Options);
			return mainWindowVm.DialogResult;
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

			if (projectInfo != null)
			{
				foreach (var targetLanguage in projectInfo.TargetLanguages)
				{
					var languagePair = new LanguagePair(projectInfo.SourceLanguage.CultureInfo, targetLanguage.CultureInfo);
					languagePairs.Add(languagePair);
				}
			}
			var options = new GTPTranslationOptions();
			var mainWindowVm = ShowProviderWindow(languagePairs.ToArray(), credentialStore, options);

			if (!mainWindowVm.DialogResult) return false;
			return mainWindowVm.DialogResult;
		}

		/// <summary>
		/// Used for displaying the plug-in info such as the plug-in name,
		/// tooltip, and icon.
		/// </summary>
		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderDisplayInfo();
			info.TranslationProviderIcon = PluginResources.my_icon;
			var options = new GTPTranslationOptions(translationProviderUri);

			if (options.SelectedProvider == ProviderType.GoogleTranslate)
			{
				if (options.SelectedGoogleVersion == ApiVersion.V2)
				{
					info.Name = PluginResources.GoogleBasic;
					info.TooltipText = PluginResources.GoogleBasic;
				}
				else
				{
					info.Name = PluginResources.GoogleAdvanced;
					info.TooltipText = PluginResources.GoogleAdvanced;
				}
				info.SearchResultImage = PluginResources.my_image;
			}
			else
			{
				info.Name = PluginResources.Plugin_NiceName;
				info.TooltipText = PluginResources.Plugin_Tooltip;
			}

			return info;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}
			return string.Equals(translationProviderUri.Scheme, Provider.ListTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
		}

		private MainWindowViewModel ShowProviderWindow(LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions)
		{
			SetSavedCredentialsOnUi(credentialStore, loadOptions);

			var dialogService = new OpenFileDialogService();
			var providerControlVm = new ProviderControlViewModel(loadOptions);
			var htmlUtil = new HtmlUtil();

			var settingsControlVm = new SettingsControlViewModel(loadOptions, dialogService, false);
			var mainWindowVm = new MainWindowViewModel(
				loadOptions, providerControlVm, settingsControlVm, credentialStore, languagePairs, htmlUtil);

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowVm
			};

			mainWindowVm.CloseEventRaised += () =>
			{
				UpdateProviderCredentials(credentialStore, loadOptions);

				mainWindow.Close();
			};

			mainWindow.ShowDialog();
			return mainWindowVm;
		}

		private void UpdateProviderCredentials(ITranslationProviderCredentialStore credentialStore,
			ITranslationOptions options)
		{
			switch (options.SelectedProvider)
			{
				case ProviderType.GoogleTranslate:
					SetCredentialsOnCredentialStore(credentialStore, PluginResources.UriGt, options.ApiKey,
						options.PersistGoogleKey);
					break;
			}
		}

		/// <summary>
		/// Get saved key if there is one and put it into options
		/// </summary>
		private void SetSavedCredentialsOnUi(ITranslationProviderCredentialStore credentialStore,
			ITranslationOptions loadOptions)
		{
			//get google credentials
			var getCredGt = GetCredentialsFromStore(credentialStore, PluginResources.UriGt);
			if (getCredGt != null)
			{
				loadOptions.ApiKey = getCredGt.Credential;
				loadOptions.PersistGoogleKey = getCredGt.Persist;
			}
		}

		private TranslationProviderCredential GetCredentialsFromStore(ITranslationProviderCredentialStore credentialStore, string uri)
		{
			var myUri = new Uri(uri);
			TranslationProviderCredential cred = null;

			if (credentialStore.GetCredential(myUri) != null)
			{
				//get the credential to return
				cred = new TranslationProviderCredential(credentialStore.GetCredential(myUri).Credential, credentialStore.GetCredential(myUri).Persist);
			}

			return cred;
		}

		private void SetCredentialsOnCredentialStore(ITranslationProviderCredentialStore credentialStore, string providerUri, string apiKey, bool persistKey)
		{
			var myUri = new Uri(providerUri);

			var cred = new TranslationProviderCredential(apiKey, persistKey);


			credentialStore.RemoveCredential(myUri);
			credentialStore.AddCredential(myUri, cred);
		}
	}

}
