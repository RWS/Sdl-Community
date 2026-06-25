using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace SampleTranslationProvider
{
	[TranslationProviderFactory(
		Id = "SampleTranslationProviderFactory",
		Name = "SampleTranslationProviderFactory",
		Description = "Sample Translation Provider")]
	public class SampleTranslationProviderFactory : ITranslationProviderFactory
	{
		
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception("Cannot handle URI.");
			}

			SampleTranslationProvider tp = new SampleTranslationProvider();

			return tp;
		}
		
		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException("Translation provider URI not supported.");
			}
			return string.Equals(translationProviderUri.Scheme, SampleTranslationProvider.ListTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
		}
	
		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			TranslationProviderInfo info = new TranslationProviderInfo();

			
			info.TranslationMethod = TranslationMethod.MachineTranslation;
			
			info.Name = PluginResources.Plugin_NiceName;
			
			return info;
		}
		
	}
}
