using System;
using System.Windows.Forms;
using NLog;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.Community.MTEdge.Provider.View;
using Sdl.Community.MTEdge.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.Studio
{
	[TranslationProviderWinFormsUi(Id = Constants.Provider_TranslationProviderWinFormsUi,
								   Name = Constants.Provider_TranslationProviderWinFormsUi,
								   Description = Constants.Provider_TranslationProviderWinFormsUi)]
	public class ProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public bool SupportsEditing => true;

		public string TypeName => PluginResources.Plugin_NiceName;

		public string TypeDescription => PluginResources.Plugin_Description;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var translationOptions = new TranslationOptions();
			var mainViewModel = ShowRequestedView(languagePairs, credentialStore, translationOptions);
			return mainViewModel.DialogResult ? new ITranslationProvider[] { new Provider(translationOptions) }
											  : null;
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is not Provider provider)
			{
				return false;
			}

			var mainViewModel = ShowRequestedView(languagePairs, credentialStore, provider.Options, true);
            return mainViewModel.DialogResult;
        }

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			_logger.Trace("");
			return new TranslationProviderDisplayInfo
			{
				Name = PluginResources.Plugin_NiceName,
				TranslationProviderIcon = PluginResources.icon1,
				TooltipText = PluginResources.Plugin_Tooltip,
				SearchResultImage = PluginResources.icon,
			};
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			_logger.Trace("");
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException("translationProviderUri", "URI not supported by the plug-in.");
			}

			return string.Equals(translationProviderUri.Scheme, Constants.TranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
		}

		private MainViewModel ShowRequestedView(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, TranslationOptions loadOptions, bool showSettingsView = false)
		{
            LoadCredentialStore(credentialStore, loadOptions);
            var mainWindowViewModel = new MainViewModel(loadOptions, credentialStore, languagePairs, showSettingsView);
            var mainWindowView = new MainView { DataContext = mainWindowViewModel };
            mainWindowViewModel.CloseEventRaised += () =>
            {
                UpdateCredentialStore(credentialStore, loadOptions);
                mainWindowView.Close();
            };

            mainWindowView.ShowDialog();
            return mainWindowViewModel;
        }

        private void LoadCredentialStore(ITranslationProviderCredentialStore credentialStore, TranslationOptions loadOptions)
        {

        }

        private void UpdateCredentialStore(ITranslationProviderCredentialStore credentialStore, TranslationOptions loadOptions)
		{

		}


		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			return false;
		}
	}
}