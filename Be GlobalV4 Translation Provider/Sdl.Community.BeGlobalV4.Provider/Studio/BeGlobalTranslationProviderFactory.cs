using System;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderFactory(Id = "SDLMachineTranslationCloudProviderFactory",
		Name = "SDLMachineTranslationCloudProviderFactory",
		Description = "SDL Machine Translation Cloud Provider")]
	public class BeGlobalTranslationProviderFactory : ITranslationProviderFactory
	{
		public static readonly Log Log = Log.Instance;
		[STAThread]
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var originalUri = new Uri("sdlmachinetranslationcloudprovider:///");
			var options = new BeGlobalTranslationOptions(translationProviderUri);
			if (credentialStore.GetCredential(originalUri) != null)
			{
				var credentials = credentialStore.GetCredential(originalUri);
				if (options.BeGlobalService == null)
				{
					var messageBoxService = new MessageBoxService();
					options.BeGlobalService = new BeGlobalV4Translator(options, messageBoxService, credentials);
				}
			}
			else
			{
				credentialStore.AddCredential(originalUri, new TranslationProviderCredential(originalUri.ToString(), true));
			}

			if (options.AuthenticationMethod.Equals(Constants.APICredentials) && string.IsNullOrEmpty(options.ClientId))
			{
				return new BeGlobalTranslationProvider(options);
			}
			var userInfo = options.BeGlobalService?.GetUserInformation(true);
			if (userInfo != null)
			{
				var subscriptionInfo = options.BeGlobalService?.GetLanguagePairs(userInfo.AccountId.ToString());
				options.SubscriptionInfo = subscriptionInfo;
			}
			return new BeGlobalTranslationProvider(options);
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

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderInfo
			{
				TranslationMethod = TranslationMethod.MachineTranslation,
				Name = PluginResources.Plugin_NiceName
			};
			return info;
		}
	}
}