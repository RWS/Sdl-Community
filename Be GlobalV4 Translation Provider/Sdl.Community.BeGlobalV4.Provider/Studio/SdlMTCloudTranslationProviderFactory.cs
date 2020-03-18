using System;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[TranslationProviderFactory(Id = "SDLMachineTranslationCloudProviderFactory",
		Name = "SDLMachineTranslationCloudProviderFactory",
		Description = "SDL Machine Translation Cloud Provider")]
	public class SdlMTCloudTranslationProviderFactory : ITranslationProviderFactory
	{		
		public static readonly Log Log = Log.Instance;

		[STAThread]
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var originalUri = new Uri(Constants.MTCloudUriScheme + ":///");
			var options = new SdlMTCloudTranslationOptions(translationProviderUri);

			var credentials = credentialStore.GetCredential(originalUri);
			if (credentials != null)
			{				
				var splitedCredentials = credentials.Credential?.Split('#');

				options.ClientId = splitedCredentials?.Length > 2 ? StringExtensions.Decrypt(splitedCredentials[0]) : string.Empty;
				options.ClientSecret = splitedCredentials?.Length > 2 ? StringExtensions.Decrypt(splitedCredentials[1]) : string.Empty;
				options.AuthenticationMethod = splitedCredentials?.Length == 4 ? splitedCredentials[2] : string.Empty;

				var resendDraft = splitedCredentials?.Length == 4 ? splitedCredentials[3] : string.Empty;

				if (!string.IsNullOrEmpty(resendDraft))
				{
					options.ResendDrafts = resendDraft.Equals("True");
				}

				if (options.BeGlobalService == null)
				{
					options.BeGlobalService = new SdlMTCloudTranslator(Constants.MTCloudTranslateAPIUri, options);
				}
			}
			else
			{
				credentialStore.AddCredential(originalUri, new TranslationProviderCredential(originalUri.ToString(), true));
			}

			int accountId;
			if (options.AuthenticationMethod.Equals("ClientLogin"))
			{
				accountId = options.BeGlobalService?.GetClientInformation() ?? 0;
			}
			else
			{
				accountId = options.BeGlobalService?.GetUserInformation() ?? 0;
			}

			var subscriptionInfo = options.BeGlobalService?.GetLanguagePairs(accountId.ToString());
			options.SubscriptionInfo = subscriptionInfo;

			return new SdlMTCloudTranslationProvider(options);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = string.Equals(translationProviderUri.Scheme, Constants.MTCloudUriScheme, StringComparison.OrdinalIgnoreCase);
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