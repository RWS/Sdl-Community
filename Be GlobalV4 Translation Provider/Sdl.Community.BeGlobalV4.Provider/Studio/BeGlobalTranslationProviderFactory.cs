using System;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderFactory(Id = "BeGlobalTranslationProviderFactory",
		Name = "BeGlobalTranslationProviderFactory",
		Description = "BeGlobal4 Translation Provider")]
	public class BeGlobalTranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var originalUri = new Uri("beglobaltranslationprovider:///");
			var options = new BeGlobalTranslationOptions(translationProviderUri);

			if (credentialStore.GetCredential(originalUri) != null)
			{
				var credentials = credentialStore.GetCredential(originalUri);
				var splitedCredentials = credentials.Credential.Split('#');
				options.ClientId = splitedCredentials[0];
				options.ClientSecret = splitedCredentials[1];
			}
			else
			{
				credentialStore.AddCredential(originalUri, new TranslationProviderCredential(originalUri.ToString(), true));
			}

			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", options.ClientId,
				options.ClientSecret, string.Empty, string.Empty, options.Model, options.UseClientAuthentication);

			int accountId;
			if (options.UseClientAuthentication)
			{
				accountId = beGlobalTranslator.GetClientInformation();
			}
			else
			{
				accountId = beGlobalTranslator.GetUserInformation();
			}
			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
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