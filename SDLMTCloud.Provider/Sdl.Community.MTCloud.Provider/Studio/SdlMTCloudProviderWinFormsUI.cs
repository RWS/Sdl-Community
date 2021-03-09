using System;
using System.Linq;
using System.Windows.Interop;
using NLog;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using IWin32Window = System.Windows.Forms.IWin32Window;
using LogManager = NLog.LogManager;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[TranslationProviderWinFormsUi(
		Id = "SDLMachineTranslationCloudProviderUi",
		Name = "SDLMTCloud_Provider_Name",
		Description = "SDLMTCloud_Provider_Description")]
	public class SdlMTCloudProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public bool SupportsEditing => true;
		public string TypeDescription => PluginResources.Plugin_NiceName;
		public string TypeName => PluginResources.Plugin_NiceName;

		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			//TODO: Instantiate the new Rate it View part
			try
			{
				var uri = new Uri($"{Constants.MTCloudUriScheme}://");
				var connectionService = new ConnectionService(owner, new VersionService(), StudioInstance.GetLanguageCloudIdentityApi(), MtCloudApplicationInitializer.Client);

				var credential = connectionService.GetCredential(credentialStore);
				var connectionResult = connectionService.EnsureSignedIn(credential, true);

				if (!connectionResult.Item1)
				{
					throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
				}

				connectionService.SaveCredential(credentialStore);

				var editorController = StudioInstance.GetEditorController();
				MtCloudApplicationInitializer.SetTranslationService(connectionService);

				var languageProvider = new LanguageProvider();
				var provider = new SdlMTCloudTranslationProvider(uri, string.Empty, MtCloudApplicationInitializer.TranslationService,
					languageProvider,
					editorController, true);

				return new ITranslationProvider[] { provider };
			}
			catch (Exception e)
			{
				_logger.Error($"{Constants.Browse} {e.Message}\n {e.StackTrace}");
				throw;
			}
		}

		[STAThread]
		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				if (!(translationProvider is SdlMTCloudTranslationProvider provider))
				{
					return false;
				}

				provider.TranslationService.ConnectionService.Owner = owner;
				var connectionResult = provider.TranslationService.ConnectionService.EnsureSignedIn(provider.TranslationService.ConnectionService.Credential);

				if (!connectionResult.Item1)
				{
					throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
				}

				provider.TranslationService.ConnectionService.SaveCredential(credentialStore);

				var optionsWindow = GetOptionsWindow(owner);
				var optionsViewModel = new OptionsViewModel(optionsWindow, provider, languagePairs.ToList());
				optionsWindow.DataContext = optionsViewModel;

				optionsWindow.ShowDialog();
				if (optionsWindow.DialogResult.HasValue && optionsWindow.DialogResult.Value)
				{
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
				Name = PluginResources.Plugin_NiceName,
				TooltipText = PluginResources.Plugin_NiceName,
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

		private static OptionsWindow GetOptionsWindow(IWin32Window owner)
		{
			var window = new OptionsWindow();
			var helper = new WindowInteropHelper(window)
			{
				Owner = owner.Handle
			};
			return window;
		}
	}
}