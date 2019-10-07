using System;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalTranslationProvider: ITranslationProvider
	{
		public static readonly string ListTranslationProviderScheme = "machinetranslationcloudprovider";
		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, "Machine Translation Cloud");
		public Uri Uri => Options.Uri;
		public string Name => "Machine Translation Cloud Provider";
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
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;

		public BeGlobalTranslationProvider(BeGlobalTranslationOptions options)
		{
			Options = options;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			try
			{
				var sourceLanguage =
					_normalizeSourceTextHelper.GetCorespondingLangCode(languageDirection.SourceCulture);
				var targetLanguage =
					_normalizeSourceTextHelper.GetCorespondingLangCode(languageDirection.TargetCulture);

				if (Options?.SubscriptionInfo?.LanguagePairs?.Count > 0)
				{
					var languagePair =
						Options.SubscriptionInfo.LanguagePairs.FirstOrDefault(
							l => l.SourceLanguageId.Equals(sourceLanguage) && l.TargetLanguageId.Equals(targetLanguage));
					if (languagePair != null)
					{
						return true;
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"SupportsLanguageDirection: {e.Message}\n {e.StackTrace}");
			}
			return false;
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
			return JsonConvert.SerializeObject(Options);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<BeGlobalTranslationOptions>(translationProviderState);
		}


	}
}
