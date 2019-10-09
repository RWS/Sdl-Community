using System;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderFactory(Id = "MachineTranslationCloudProviderFactory",
		Name = "MachineTranslationCloudProviderFactory",
		Description = "Machine Translation Cloud Provider")]
	public class BeGlobalTranslationProviderFactory : ITranslationProviderFactory
	{
		public static readonly Log Log = Log.Instance;	
		[STAThread]
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			AppItializer.EnsureInitializer();

			var originalUri = new Uri("machinetranslationcloudprovider:///");
			var options = new BeGlobalTranslationOptions(translationProviderUri);
			if (credentialStore.GetCredential(originalUri) != null)
			{
				var credentials = credentialStore.GetCredential(originalUri);
				var splitedCredentials = credentials.Credential.Split('#');
				options.ClientId = splitedCredentials[0];
				options.ClientSecret = splitedCredentials[1];
				if (options.BeGlobalService == null)
				{
					var messageBoxService = new MessageBoxService();
					options.BeGlobalService = new BeGlobalV4Translator(options, messageBoxService);
				}
			}
			else
			{
				credentialStore.AddCredential(originalUri, new TranslationProviderCredential(originalUri.ToString(), true));
			}
			var accountId = options.BeGlobalService.GetUserInformation();		
			var subscriptionInfo = options.BeGlobalService.GetLanguagePairs(accountId.ToString());
			options.SubscriptionInfo = subscriptionInfo;
			return new BeGlobalTranslationProvider(options);

			//if (options.BeGlobalService == null)
			//{
			//	options.BeGlobalService = new BeGlobalV4Translator(options);
			//}
			//try
			//{
			//	var accountId = options.BeGlobalService.GetUserInformation();
			//	var subscriptionInfo = options.BeGlobalService.GetLanguagePairs(accountId.ToString());
			//	options.SubscriptionInfo = subscriptionInfo;
			//}
			//catch (Exception e)
			//{
			//	Log.Logger.Error(e, "Error on CreateTranslationProvider");
			//}

			//return new BeGlobalTranslationProvider(options);
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