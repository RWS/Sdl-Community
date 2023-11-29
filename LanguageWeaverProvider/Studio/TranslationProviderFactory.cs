using System;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Options;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[TranslationProviderFactory(Id = Constants.Provider_TranslationProviderFactory,
								Name = Constants.Provider_TranslationProviderFactory,
								Description = Constants.Provider_TranslationProviderFactory)]
	internal class TranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			ApplicationInitializer.CredentialStore = credentialStore;
			var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			CredentialManager.GetCredentials(options, true);
			return new TranslationProvider(options);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			var pluginName = string.IsNullOrEmpty(translationProviderState)
						   ? Constants.PluginName
						   : StringExtensions.GetPluginName(translationProviderState);

			return new TranslationProviderInfo
			{
				TranslationMethod = TranslationMethod.MachineTranslation,
				Name = pluginName
			};
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			var supportsTranslationProviderUri = translationProviderUri switch
			{
				null => throw new ArgumentNullException("Unsuported"),
				_ => translationProviderUri.Scheme.StartsWith(Constants.BaseTranslationScheme)
			};

			return supportsTranslationProviderUri;
		}
	}
}