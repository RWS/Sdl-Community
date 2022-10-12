using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using MTEnhancedMicrosoftProvider.Interfaces;
using MTEnhancedMicrosoftProvider.Model;
using MTEnhancedMicrosoftProvider.Service;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;
using MTEnhancedMicrosoftProvider.View;
using MTEnhancedMicrosoftProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace MTEnhancedMicrosoftProvider.Studio
{
    [TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
                                   Name = "Translation_Provider_Plug_inWinFormsUI",
                                   Description = "Translation_Provider_Plug_inWinFormsUI")]
    public class MTEMicrosoftProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		public string TypeDescription => PluginResources.Plugin_Description;
		public string TypeName => PluginResources.Plugin_NiceName;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			var options = new MTEMicrosoftTranslationOptions();
			var regionsProvider = new RegionsProvider();
			var htmlUtil = new HtmlUtil();

			var mainWindowVm = ShowProviderWindow(languagePairs, credentialStore, options, regionsProvider);

			if (!mainWindowVm.DialogResult) return null;

			var provider = new MTEMicrosoftProvider(options, regionsProvider, htmlUtil);

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
			if (!(translationProvider is MTEMicrosoftProvider editProvider))
			{
				return false;
			}

			var mainWindowVm = ShowProviderWindow(languagePairs, credentialStore, editProvider.Options, editProvider.RegionsProvider);
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
			var options = new MTEMicrosoftTranslationOptions();
			var regionsProvider = new RegionsProvider();
			var mainWindowVm = ShowProviderWindow(languagePairs.ToArray(), credentialStore, options, regionsProvider);

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
			var options = new MTEMicrosoftTranslationOptions(translationProviderUri);
			info.TranslationProviderIcon = PluginResources.my_icon;

			if (options.SelectedProvider == MTEMicrosoftTranslationOptions.ProviderType.MicrosoftTranslator)
			{
				info.Name = PluginResources.Microsoft_NiceName;
				info.TooltipText = PluginResources.Microsoft_Tooltip;
				info.SearchResultImage = PluginResources.microsoft_image;
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
			return string.Equals(translationProviderUri.Scheme, MTEMicrosoftProvider.ListTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
		}

		private MainWindowViewModel ShowProviderWindow(LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions, RegionsProvider regionsProvider)
		{
			SetSavedCredentialsOnUi(credentialStore, loadOptions);

			var dialogService = new OpenFileDialogService();
			var providerControlVm = new ProviderControlViewModel(loadOptions, regionsProvider);
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

		private void UpdateProviderCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions options)
		{

			SetCredentialsOnCredentialStore(credentialStore, PluginResources.UriMs, options.ClientId,
				options.PersistMicrosoftCreds);
		}

		/// <summary>
		/// Get saved key if there is one and put it into options
		/// </summary>
		private void SetSavedCredentialsOnUi(ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions)
		{
			//get microsoft credentials
			var getCredMt = GetCredentialsFromStore(credentialStore, PluginResources.UriMs);
			if (getCredMt != null)
			{
				try
				{
					loadOptions.ClientId = getCredMt.Credential;
					loadOptions.PersistMicrosoftCreds = getCredMt.Persist;
				}
				catch (Exception ex) //swallow b/c it will just fail to fill in instead of crashing the whole program
				{
					_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");
				}
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
