using System;
using System.Collections.Generic;
using System.Net;
using MicrosoftTranslatorProvider.Model;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
	[TranslationProviderFactory(Id = "MicrosoftTranslatorProviderPlugin_Factory",
                                Name = "MicrosoftTranslatorProviderPlugin_Factory",
                                Description = "MicrosoftTranslatorProviderPlugin_Factory")]
	public class ProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var credential = credentialStore.GetCredential(new Uri(Constants.MicrosoftProviderFullScheme)) ?? throw new TranslationProviderAuthenticationException();
			var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			var privateHeaders = new List<UrlMetadata>();
			try
			{
				var genericCredentials = new GenericCredentials(credential.Credential);
				foreach (var credentialKey in genericCredentials.GetPropertyKeys())
				{
					if (!credentialKey.StartsWith("header_"))
					{
						continue;
					}

					privateHeaders.Add(new UrlMetadata()
					{
						Key = credentialKey.Replace("header_", string.Empty),
						Value = genericCredentials[credentialKey]
					});
				}

				options.ApiKey = genericCredentials["API-Key"];
				options.PrivateEndpoint ??= genericCredentials["PrivateEndpoint"];
			}
			catch { }

			return new Provider(options) { PrivateHeaders = privateHeaders };
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				TranslationMethod = TranslationOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_NiceName
			};
		}
	}
}