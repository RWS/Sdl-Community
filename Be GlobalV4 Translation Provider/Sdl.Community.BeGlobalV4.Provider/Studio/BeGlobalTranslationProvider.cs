using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class BeGlobalTranslationProvider: ITranslationProvider
	{
		private LanguagePair _languageDirection;

		public static readonly string ListTranslationProviderScheme = "sdlmachinetranslationcloudprovider";
		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, Constants.PluginName);
		public Uri Uri => Options.Uri;
		public string Name => Constants.PluginName;
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
		private LanguageMappingsService _languageMappingsService;
		private string _encryptedClientId;
		private string _encryptedClientSecret;

		public BeGlobalTranslationProvider(BeGlobalTranslationOptions options)
		{
			Options = options;
			_languageMappingsService = new LanguageMappingsService();
			_encryptedClientId = StringExtensions.EncryptData(Options.ClientId);
			_encryptedClientSecret = StringExtensions.EncryptData(Options.ClientSecret);
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			try
			{
				_languageDirection = languageDirection;
				var languagePair = SetSupportedLanguages(_languageDirection);
				if (languagePair != null)
				{
					return true;
				}				
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.SupportsLanguageDirection} {e.Message}\n {e.StackTrace}");
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
			SetSupportedLanguages(_languageDirection);
			Options.ClientId = _encryptedClientId;
			Options.ClientSecret = _encryptedClientSecret;
			return JsonConvert.SerializeObject(Options);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<BeGlobalTranslationOptions>(translationProviderState);
		}

		private BeGlobalLanguagePair SetSupportedLanguages(LanguagePair languageDirection)
		{
			var languageMappings = _languageMappingsService.GetLanguageMappingSettings()?.LanguageMappings;
			if (languageDirection != null && languageMappings != null)
			{				
				if (Options?.SubscriptionInfo?.LanguagePairs?.Count > 0 && (languageMappings != null || languageMappings.Any()))
				{
					var languagePair = Options.SubscriptionInfo.LanguagePairs
						.FirstOrDefault(o => languageMappings
									   .Any(l => l.SelectedMTCodeSource.CodeName.Equals(o.SourceLanguageId) && l.SelectedMTCodeTarget.CodeName.Equals(o.TargetLanguageId)));
					if (languagePair != null)
					{
						Options.LanguagesSupported = new Dictionary<string, string>() { { languageDirection.TargetCulture.Name, languagePair.Name } };
					}
					return languagePair;
				}
			}
			return new BeGlobalLanguagePair();
		}
	}
}