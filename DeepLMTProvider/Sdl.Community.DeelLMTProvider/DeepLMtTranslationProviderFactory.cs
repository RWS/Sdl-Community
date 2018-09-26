using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using Sdl.Community.DeepLMTProvider.WPF.Model;

namespace Sdl.Community.DeepLMTProvider
{
	[TranslationProviderFactory(Id = "DeepLMtTranslationProviderFactory",
		Name = "DeepLMtTranslationProviderFactory",
		Description = "DeepL Mt Translation Provider")]
	public class DeepLMtTranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			var originalUri = new Uri("deeplprovider:///");
			var options = new DeepLTranslationOptions(translationProviderUri);

			if (credentialStore.GetCredential(originalUri) != null)
			{
				var credentials = credentialStore.GetCredential(originalUri);
				options.ApiKey = credentials.Credential;
			}

			// create identifier on first use and save as a credential
			originalUri = new Uri("deeplproviderIdentifier:///");
			if (credentialStore.GetCredential(originalUri) == null)
			{
				credentialStore.AddCredential(originalUri, new TranslationProviderCredential(Guid.NewGuid().ToString(), true));
			}
			if (credentialStore.GetCredential(originalUri) != null)
			{
				var credentials = credentialStore.GetCredential(originalUri);
				options.Identifier = credentials.Credential;
			}
			return new DeepLMtTranslationProvider(options);
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

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = string.Equals(translationProviderUri.Scheme, DeepLMtTranslationProvider.ListTranslationProviderScheme,
				StringComparison.OrdinalIgnoreCase);
			return supportsProvider;
		}
	}
}