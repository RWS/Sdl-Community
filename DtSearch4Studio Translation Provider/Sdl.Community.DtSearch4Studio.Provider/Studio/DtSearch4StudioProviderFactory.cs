using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider.Studio
{
	[TranslationProviderFactory(Id = "DtSearch4StudioFactoryId", Name = "DtSearch4StudioFactory", Description = "DtSearch4Studio Translation Provider Factory")]
	public class DtSearch4StudioProviderFactory : ITranslationProviderFactory
	{
		public static readonly Log Log = Log.Instance;

		#region ITranslationProviderFactory Members
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			DtSearch4StudioProvider dtSearch4StudioProvider;
			try
			{
				var persistenceService = new PersistenceService();
				var providerSettings = persistenceService.GetProviderSettings();

				// in case we didn't have any settings stored there is no need to load the provider
				if (providerSettings == null)
				{
					MessageBox.Show(Constants.EmptyProvider, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return null;
				}				
				//var searchService = new SearchService();
				dtSearch4StudioProvider = new DtSearch4StudioProvider(providerSettings);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.CreateTranslationProvider}: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
			return dtSearch4StudioProvider;
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				Name = "DtSearch4Studio",
				TranslationMethod =  TranslationMethod.MachineTranslation
			};
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return translationProviderUri.Scheme == Constants.ProviderScheme;
		}
		#endregion
	}
}