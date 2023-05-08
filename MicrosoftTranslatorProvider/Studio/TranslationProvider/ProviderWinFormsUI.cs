using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.View;
using MicrosoftTranslatorProvider.ViewModel;
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

		private MainWindowViewModel ShowProviderWindow(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions, RegionsProvider regionsProvider, bool showSettingsView = false)
		{
			var dialogService = new OpenFileDialogService();
			var providerControlViewModel = new ProviderControlViewModel(loadOptions, regionsProvider);
			var settingsControlViewModel = new SettingsControlViewModel(loadOptions, credentialStore, dialogService, false);
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
