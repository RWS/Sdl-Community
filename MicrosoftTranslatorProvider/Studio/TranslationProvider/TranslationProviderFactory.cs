using System;
using System.Threading.Tasks;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
	[TranslationProviderFactory(Id = "MicrosoftTranslatorProviderPlugin_Factory",
                                Name = "MicrosoftTranslatorProviderPlugin_Factory",
                                Description = "MicrosoftTranslatorProviderPlugin_Factory")]
	public class TranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			ApplicationInitializer.CredentialStore = credentialStore;
			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			CredentialsManager.GetCredentials(translationOptions);
			if (translationOptions.AuthenticationType == AuthenticationType.Microsoft)
			{
				_ = Task.Run(async () => await MicrosoftService.AuthenticateUser(translationOptions.MicrosoftCredentials)).Result;
			}

			return new TranslationProvider(translationOptions);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderScheme, StringComparison.OrdinalIgnoreCase)
				|| string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderPrivateEndpointScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				TranslationMethod = TranslationMethod.MachineTranslation,
				Name = PluginResources.Plugin_NiceName
			};
		}
	}
}