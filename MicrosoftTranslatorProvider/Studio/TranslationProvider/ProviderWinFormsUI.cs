using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.View;
using MicrosoftTranslatorProvider.ViewModel;
using NLog;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace MicrosoftTranslatorProvider.Studio
{
	[TranslationProviderWinFormsUi(Id = "MicrosoftTranslatorProviderPlugin_WinFormsUI",
								   Name = "MicrosoftTranslatorProviderPlugin_WinFormsUI",
                                   Description = "MicrosoftTranslatorProviderPlugin_WinFormsUI")]
    public class ProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public string TypeDescription => PluginResources.Plugin_Description;
		public string TypeName => PluginResources.Plugin_NiceName;
		public bool SupportsEditing => true;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var options = new MTETranslationOptions();
			var regionsProvider = new RegionsProvider();
			var mainWindowDialogResult = ShowProviderWindow(languagePairs, credentialStore, options, regionsProvider).DialogResult;

			var htmlUtil = new HtmlUtil();
			return mainWindowDialogResult ? new ITranslationProvider[] { new Provider(options, regionsProvider, htmlUtil) }
										  : null;
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is not Provider editProvider)
			{
				return false;
			}

			var mainWindowViewModel = ShowProviderWindow(languagePairs, credentialStore, editProvider.Options, editProvider.RegionsProvider, true);
			return mainWindowViewModel.DialogResult;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var options = new MTETranslationOptions(translationProviderUri);
			var customName = options.CustomProviderName;
			var useCustomName = options.UseCustomProviderName;
			var providerName = customName.SetProviderName(useCustomName);
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

			return string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderScheme, StringComparison.CurrentCultureIgnoreCase);
		}

		private MainWindowViewModel ShowProviderWindow(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions, RegionsProvider regionsProvider, bool showSettingsView = false)
		{
			var dialogService = new OpenFileDialogService();
			var providerControlViewModel = new ProviderControlViewModel(loadOptions, regionsProvider);
			var settingsControlViewModel = new SettingsControlViewModel(loadOptions, dialogService, false);
			var htmlUtil = new HtmlUtil();
			var mainWindowViewModel = new MainWindowViewModel(loadOptions,
															  providerControlViewModel,
															  settingsControlViewModel,
															  credentialStore,
															  languagePairs,
															  htmlUtil,
															  showSettingsView);
			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

			mainWindowViewModel.CloseEventRaised += () =>
			{
				mainWindow.Close();
			};

			mainWindow.ShowDialog();
			return mainWindowViewModel;
		}

		private void UpdateProviderCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions options)
		{
			SetCredentialsOnCredentialStore(credentialStore, options.ClientID, options.PersistMicrosoftCredentials);
			SetPrivateEndpointOnCredentialStore(credentialStore, options.PrivateEndpoint, options.PersistPrivateEndpoint);
		}

		private void SetSavedCredentialsOnUi(ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions)
		{
			if (GetCredentialsFromStore(credentialStore, Constants.MicrosoftProviderFullScheme)
				is not TranslationProviderCredential providerCredentials)
			{
				return;
			}

			try
			{
				loadOptions.ClientID = providerCredentials.Credential;
				loadOptions.PersistMicrosoftCredentials = providerCredentials.Persist;

				if (GetCredentialsFromStore(credentialStore, Constants.MicrosoftProviderPrivateEndpointScheme)
					is TranslationProviderCredential privateEndpoint)
				{
					loadOptions.PrivateEndpoint = privateEndpoint.Credential;
					loadOptions.PersistPrivateEndpoint = privateEndpoint.Persist;
				}

			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {e.Message}\n {e.StackTrace}");
			}
		}

		private TranslationProviderCredential GetCredentialsFromStore(ITranslationProviderCredentialStore credentialStore, string uri)
		{
			var providerCredentials = credentialStore.GetCredential(new Uri(uri));
			return providerCredentials != null
				 ? new TranslationProviderCredential(providerCredentials.Credential, providerCredentials.Persist)
				 : null;
		}

		private void SetCredentialsOnCredentialStore(ITranslationProviderCredentialStore credentialStore, string apiKey, bool persistKey)
		{
			var uri = new Uri(Constants.MicrosoftProviderFullScheme);
			var proiderCredentials = new TranslationProviderCredential(apiKey, persistKey);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, proiderCredentials);
		}

		private void SetPrivateEndpointOnCredentialStore(ITranslationProviderCredentialStore credentialStore, string privateEndpoint, bool persist)
		{
			var uri = new Uri(Constants.MicrosoftProviderPrivateEndpointScheme);
			var proiderCredentials = new TranslationProviderCredential(privateEndpoint, persist);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, proiderCredentials);
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

			var options = new MTETranslationOptions();
			var regionsProvider = new RegionsProvider();
			var mainWindowViewModel = ShowProviderWindow(languagePairs.ToArray(), credentialStore, options, regionsProvider);
			return mainWindowViewModel.DialogResult;
		}
	}
}
