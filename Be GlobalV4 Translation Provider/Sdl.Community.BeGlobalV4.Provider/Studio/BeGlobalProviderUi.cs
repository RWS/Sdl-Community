using System;
using System.Windows.Forms;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderWinFormsUi(
		Id = "SDLMachineTranslationCloudProviderUi",
		Name = "SDLMachineTranslationCloudProviderUi",
		Description = "SDL Machine Translation Cloud Provider")]
	public class BeGlobalProviderUi : ITranslationProviderWinFormsUI
	{
		public string TypeName => Constants.PluginName;
		public string TypeDescription => Constants.PluginName;
		public bool SupportsEditing => true;
		public static readonly Log Log = Log.Instance;

		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var options = new BeGlobalTranslationOptions();
				var token = string.Empty;
				
				var credentials = GetCredentials(credentialStore, "sdlmachinetranslationcloudprovider:///");				
				AppItializer.EnsureInitializer();

				var beGlobalWindow = new BeGlobalWindow();
				var beGlobalVm = new BeGlobalWindowViewModel(options, languagePairs, credentials);
				beGlobalWindow.DataContext = beGlobalVm;

				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					var messageBoxService = new MessageBoxService();
					var beGlobalService = new BeGlobalV4Translator(beGlobalVm.Options, messageBoxService, credentials);
					beGlobalVm.Options.BeGlobalService = beGlobalService;

					var provider = new BeGlobalTranslationProvider(options)
					{
						Options = beGlobalVm.Options
					};

					SetCredentials(credentialStore, beGlobalVm.Options.ClientId, beGlobalVm.Options.ClientSecret, true);
					return new ITranslationProvider[] { provider };
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.Browse} {e.Message}\n {e.StackTrace}");
			}
			return null;
		}

		[STAThread]
		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var editProvider = translationProvider as BeGlobalTranslationProvider;

				if (editProvider == null)
				{
					return false;
				}

				//get saved key if there is one and put it into options
				var credentials = GetCredentials(credentialStore, "sdlmachinetranslationcloudprovider:///");
				if (credentials != null)
				{
					var splitedCredentials = credentials.Credential.Split('#');
					if (splitedCredentials.Length == 2 && !string.IsNullOrEmpty(splitedCredentials[0]) && !string.IsNullOrEmpty(splitedCredentials[1]))
					{
						editProvider.Options.ClientId = splitedCredentials[0];
						editProvider.Options.ClientSecret = splitedCredentials[1];
					}
				}
				var beGlobalWindow = new BeGlobalWindow();
				var beGlobalVm = new BeGlobalWindowViewModel(editProvider.Options, languagePairs, credentials);
				beGlobalWindow.DataContext = beGlobalVm;

				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					editProvider.Options = beGlobalVm.Options;
					editProvider.Options.ClientId = beGlobalWindow.LoginTab.ClientIdBox.Password.TrimEnd();
					editProvider.Options.ClientSecret = beGlobalWindow.LoginTab.ClientSecretBox.Password.TrimEnd();
					SetCredentials(credentialStore, editProvider.Options.ClientId, beGlobalVm.Options.ClientSecret, true);
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

			var supportsProvider = string.Equals(translationProviderUri.Scheme, BeGlobalTranslationProvider.ListTranslationProviderScheme,
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
			throw new NotImplementedException();
		}

		private TranslationProviderCredential GetCredentials(ITranslationProviderCredentialStore credentialStore, string uri)
		{
			var providerUri = new Uri(uri);
			TranslationProviderCredential cred = null;
			if (credentialStore.GetCredential(providerUri) != null)
			{
				//get the credential to return				
				cred = new TranslationProviderCredential(credentialStore.GetCredential(providerUri)?.Credential, credentialStore.GetCredential(providerUri).Persist);
			}
			return cred;
		}

		private void SetCredentials(ITranslationProviderCredentialStore credentialStore, string clientId, string clientSecret, bool persistKey)
		{
			var uri = new Uri("sdlmachinetranslationcloudprovider:///");
			var credential = $"{clientId}#{clientSecret}";
			var credentials = new TranslationProviderCredential(credential, persistKey);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, credentials);
		}
	}
}