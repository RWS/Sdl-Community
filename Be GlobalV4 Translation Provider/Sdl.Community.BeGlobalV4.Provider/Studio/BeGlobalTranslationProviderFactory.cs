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
		private string _url = "https://translate-api.sdlbeglobal.com";
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var originalUri = new Uri("sdlmachinetranslationcloudprovider:///");
			var options = new BeGlobalTranslationOptions(translationProviderUri);

			if (credentialStore.GetCredential(originalUri) != null)
			{
				var credentials = credentialStore.GetCredential(originalUri);
				var splitedCredentials = credentials.Credential.Split('#');
				options.ClientId = StringExtensions.Base64Decode(splitedCredentials[0]);
				options.ClientSecret = StringExtensions.Base64Decode(splitedCredentials[1]);
				if (options.BeGlobalService == null)
				{
					options.BeGlobalService = new BeGlobalV4Translator(_url, options.ClientId, options.ClientSecret, options.Model, options.UseClientAuthentication);
				}
			}
			else
			{
				credentialStore.AddCredential(originalUri, new TranslationProviderCredential(originalUri.ToString(), true));
			}

			int accountId;
			if (options.UseClientAuthentication)
			{
				accountId = options.BeGlobalService.GetClientInformation();
			}
			else
			{
				accountId = options.BeGlobalService.GetUserInformation();
			}

			var subscriptionInfo = options.BeGlobalService.GetLanguagePairs(accountId.ToString());
			options.SubscriptionInfo = subscriptionInfo;
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