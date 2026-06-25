using System;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace SampleTranslationProvider
{
	public class SampleTranslationProvider : ITranslationProvider
	{
		 
		public static readonly string ListTranslationProviderScheme = "sampletranslationprovider";

        readonly TranslationProviderUriBuilder _uriBuilder;


        public SampleTranslationProvider()
		{
            _uriBuilder = new TranslationProviderUriBuilder(SampleTranslationProvider.ListTranslationProviderScheme);
        }
		

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new SampleTranslationProviderLanguageDirection(this, languageDirection);
		}

		public bool IsReadOnly => true;

        public void LoadState(string translationProviderState)
		{
		}

		public string Name => PluginResources.Plugin_NiceName;

        public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			return null;
		}

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Plugin_NiceName);

        public bool SupportsConcordanceSearch => true;

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return true;
        }
        
	
		public bool SupportsSearchForTranslationUnits => true;

       
		public bool SupportsSourceConcordanceSearch => true;

		public bool SupportsTargetConcordanceSearch => true;

		public bool SupportsUpdate => false;

		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;


        public Uri Uri => _uriBuilder.Uri;
    }
}

