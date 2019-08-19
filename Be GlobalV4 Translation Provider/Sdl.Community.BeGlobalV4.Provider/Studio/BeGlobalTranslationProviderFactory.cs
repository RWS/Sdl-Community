using System;
using System.Net;
using System.Windows;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderFactory(Id = "BeGlobalTranslationProviderFactory",
		Name = "BeGlobalTranslationProviderFactory",
		Description = "BeGlobal4 Translation Provider")]
	public class BeGlobalTranslationProviderFactory : ITranslationProviderFactory
	{
		public static readonly Log Log = Log.Instance;	
		[STAThread]
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			AppItializer.EnsureInitializer();

			var options = new BeGlobalTranslationOptions(translationProviderUri);
			if (options.BeGlobalService == null)
			{
				options.BeGlobalService = new BeGlobalV4Translator(options.Model);
			}
			try
			{
				var accountId = options.BeGlobalService.GetUserInformation();
				var subscriptionInfo = options.BeGlobalService.GetLanguagePairs(accountId.ToString());

				options.SubscriptionInfo = subscriptionInfo;
			}
			catch (Exception e)
			{
				Log.Logger.Error(e, "Error on CreateTranslationProvider");
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