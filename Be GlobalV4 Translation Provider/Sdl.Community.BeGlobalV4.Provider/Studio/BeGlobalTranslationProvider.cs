using System;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalTranslationProvider: ITranslationProvider
	{
		public static readonly string ListTranslationProviderScheme = "beglobaltranslationprovider";
		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, "SDL BeGlobal (NMT)");
		public Uri Uri => Options.Uri;
		public string Name => "BeGlobal4 Translation Provider";
		public bool SupportsTaggedInput => true;  
		public bool SupportsScoring => false;	  
		public bool SupportsSearchForTranslationUnits => true; 
		public bool SupportsMultipleResults => false;  
		public bool SupportsFilters => false;	
		public bool SupportsPenalties => true;	 
		public bool SupportsStructureContext => false;	
		public bool SupportsDocumentSearches => false;	
		public bool SupportsUpdate => false;	  
		public bool SupportsPlaceables => false;	 
		public bool SupportsTranslation => true;	
		public bool SupportsFuzzySearch => false;		 
		public bool SupportsConcordanceSearch => false;	  
		public bool SupportsSourceConcordanceSearch => false; 
		public bool SupportsTargetConcordanceSearch => false;	  
		public bool SupportsWordCounts => false;
		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;	 
		public bool IsReadOnly => true;
		public BeGlobalTranslationOptions Options { get; set; }

		public BeGlobalTranslationProvider(BeGlobalTranslationOptions options)
		{
			Options = options;
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			// here we need to get the language directions from Be Global
			//var normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			//var sourceLanguage =
			//	normalizeSourceTextHelper.GetCorespondingLangCode(languageDirection.SourceCulture.ThreeLetterISOLanguageName);
			//var targetLanguage =
			//	normalizeSourceTextHelper.GetCorespondingLangCode(languageDirection.TargetCulture.ThreeLetterISOLanguageName);
				
			//var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com",Options.ClientId,Options.ClientSecret,sourceLanguage,targetLanguage,Options.Model,Options.UseClientAuthentication);
			//var accountId =beGlobalTranslator.GetUserInformation();
			//var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());

			return true;
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return  new BeGlobalLanguageDirection(this,languageDirection);
		}

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			return null;
		}

		public void LoadState(string translationProviderState)
		{
		}


	}
}
