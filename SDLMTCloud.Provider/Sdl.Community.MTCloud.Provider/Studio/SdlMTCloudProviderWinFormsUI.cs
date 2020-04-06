using System;
using System.Windows.Interop;
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
		Name = "SDLMachineTranslationCloudProviderUi",
		Description = "SDL Machine Translation Cloud Provider")]
	public class SdlMTCloudProviderWinFormsUI : ITranslationProviderWinFormsUI
	{		
		public string TypeName => Constants.PluginName;
		public string TypeDescription => Constants.PluginName;
		public bool SupportsEditing => true;		
		
		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{				
				var uri = new Uri($"{Constants.MTCloudUriScheme}://");
				var connectionService = new ConnectionService(owner, new VersionService());
				
				var credential = connectionService.GetCredential(credentialStore);								
				var connectionResult = connectionService.EnsureSignedIn(credential, true);

				if (!connectionResult.Item1)
				{
					throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
				}
				
				connectionService.SaveCredential(credentialStore);

				var languageMappingsService = new LanguageMappingsService();
				var translationService = new TranslationService(connectionService, languageMappingsService);
				var editorController = SdlTradosStudio.Application?.GetController<EditorController>();

				var provider = new SdlMTCloudTranslationProvider(uri, translationService, string.Empty, editorController);				
				
				var optionsViewModel = new OptionsViewModel(null, provider, languagePairs);				
				optionsViewModel.SaveLanguageMappings();

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
				var optionsViewModel = new OptionsViewModel(optionsWindow, provider, languagePairs);
				optionsWindow.DataContext = optionsViewModel;

				optionsWindow.ShowDialog();
				if (optionsWindow.DialogResult.HasValue && optionsWindow.DialogResult.Value)
				{
					provider.TranslationService.UpdateLanguageMappings();									
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

			var supportsProvider = string.Equals(translationProviderUri.Scheme, Constants.MTCloudUriScheme,
				StringComparison.OrdinalIgnoreCase);
			return supportsProvider;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderDisplayInfo
			{
				Name = Constants.PluginName,
				TooltipText = Constants.PluginName,
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