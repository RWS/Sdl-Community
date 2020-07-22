using System;
using System.Linq;
using System.Windows.Interop;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[TranslationProviderWinFormsUi(
		Id = "SDLMachineTranslationCloudProviderUi",
		Name = "SDLMTCloud_Provider_Name",
		Description = "SDLMTCloud_Provider_Description")]
	public class SdlMTCloudProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		public string TypeName => PluginResources.Plugin_NiceName;

		public string TypeDescription => PluginResources.Plugin_NiceName;

		public bool SupportsEditing => true;		
		
		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			//TODO: Instantiate the new Rate it View part
			try
			{
				var uri = new Uri($"{Constants.MTCloudUriScheme}://");				
				var connectionService = new ConnectionService(owner, new VersionService(), StudioInstance.GetLanguageCloudIdentityApi());
				
				var credential = connectionService.GetCredential(credentialStore);								
				var connectionResult = connectionService.EnsureSignedIn(credential, true);

				if (!connectionResult.Item1)
				{
					throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
				}
				
				connectionService.SaveCredential(credentialStore);

				
				var translationService = new TranslationService(connectionService);
				var langaugeProvider = new LanguageProvider();
				var editorController = SdlTradosStudio.Application?.GetController<EditorController>();

				var provider = new SdlMTCloudTranslationProvider(uri, string.Empty, translationService, langaugeProvider, editorController);				
								
				return new ITranslationProvider[] { provider };

			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.Browse} {e.Message}\n {e.StackTrace}");
			}

			return null;
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
				Log.Logger.Error($"{Constants.EditWindow} {e.Message}\n {e.StackTrace}");
			}

			return false;
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
		
		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			return false;
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