using System;
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
			try
			{
				var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
				return new TranslationProvider(options, credentialStore);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			if (string.IsNullOrEmpty(translationProviderState))
			{
				return new TranslationProviderInfo
				{
					TranslationMethod = TranslationMethod.MachineTranslation,
					Name = Constants.PluginName
				};
			}

			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			var pluginName = translationOptions.Version == PluginVersion.LanguageWeaverCloud ? Constants.PluginNameCloud : Constants.PluginNameEdge;
			if (!string.IsNullOrEmpty(translationOptions.ProviderSettings.CustomName) && translationOptions.ProviderSettings.UseCustomName)
			{
				pluginName += $" - {translationOptions.ProviderSettings.CustomName}";
			}

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