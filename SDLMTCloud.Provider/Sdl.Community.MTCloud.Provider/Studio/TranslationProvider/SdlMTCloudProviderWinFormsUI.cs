using System;
using System.Linq;
using System.Windows.Interop;
using NLog;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using IWin32Window = System.Windows.Forms.IWin32Window;
using LogManager = NLog.LogManager;

namespace Sdl.Community.MTCloud.Provider.Studio.TranslationProvider
{
	[TranslationProviderWinFormsUi(
		Id = "SDLMachineTranslationCloudProviderUi",
		Name = "SDLMTCloud_Provider_Name",
		Description = "SDLMTCloud_Provider_Description")]
	public class SdlMTCloudProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public bool SupportsEditing => true;
		public string TypeDescription => PluginResources.SDLMTCloud_Provider_Name;
		public string TypeName => PluginResources.SDLMTCloud_Provider_Name;

		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var uri = new Uri($"{Constants.MTCloudUriScheme}://");
				var connectionService = MtCloudApplicationInitializer.ConnectionService;
				connectionService.Owner = owner;

				var connectionResult = connectionService.EnsureSignedIn(credentialStore, true);

				if (!connectionResult.Item1)
				{
					throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
				}
				connectionService.SaveCredential();

				MtCloudApplicationInitializer.SetTranslationService(connectionService, null);

				var languageProvider = new LanguageProvider();
				var provider = new SdlMTCloudTranslationProvider(uri, string.Empty, MtCloudApplicationInitializer.TranslationService,
					languageProvider);

				var optionsWindow = GetOptionsWindow(owner, languagePairs, provider);

				optionsWindow.ShowDialog();
				if (optionsWindow.DialogResult.HasValue && optionsWindow.DialogResult.Value)
				{
					MtCloudApplicationInitializer.PublishEvent(new TranslationProviderAdded());

					return new ITranslationProvider[] { provider };
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{Constants.Browse} {e.Message}\n {e.StackTrace}");
			}

			return null;
		}

		[STAThread]
		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				if (translationProvider is not SdlMTCloudTranslationProvider provider)
				{
					return false;
				}

				provider.TranslationService.ConnectionService.Owner = owner;
				var connectionResult = provider.TranslationService.ConnectionService.EnsureSignedIn(credentialStore);

				if (!connectionResult.Item1)
				{
					throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
				}

				provider.TranslationService.ConnectionService.SaveCredential();

				var optionsWindow = GetOptionsWindow(owner, languagePairs, provider);

				optionsWindow.ShowDialog();
				if (optionsWindow.DialogResult.HasValue && optionsWindow.DialogResult.Value)
				{
					var sendFeedback = false;
					if (optionsWindow.DataContext is OptionsViewModel options)
						sendFeedback = options.SendFeedback;

					MtCloudApplicationInitializer.PublishEvent(new TranslationProviderRateItOptionsChanged(sendFeedback));
					return true;
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{Constants.EditWindow} {e.Message}\n {e.StackTrace}");
			}

			return false;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			return false;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderDisplayInfo
			{
				Name = PluginResources.SDLMTCloud_Provider_Name,
				TooltipText = PluginResources.SDLMTCloud_Provider_Name,
				TranslationProviderIcon = PluginResources.global,
				SearchResultImage = PluginResources.global1,
			};
			return info;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = translationProviderUri.Scheme.StartsWith(Constants.MTCloudUriScheme);
			return supportsProvider;
		}

		private static OptionsWindow GetOptionsWindow(IWin32Window owner, LanguagePair[] languagePairs, SdlMTCloudTranslationProvider provider)
		{
			var optionsWindow = new OptionsWindow();

			var _ = new WindowInteropHelper(optionsWindow)
			{
				Owner = owner.Handle
			};

			var optionsViewModel = new OptionsViewModel(optionsWindow, provider, languagePairs.ToList());
			optionsWindow.DataContext = optionsViewModel;
			return optionsWindow;
		}
	}
}