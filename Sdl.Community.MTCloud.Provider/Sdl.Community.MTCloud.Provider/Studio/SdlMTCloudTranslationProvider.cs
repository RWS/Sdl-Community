using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class SdlMTCloudTranslationProvider : ITranslationProvider
	{
		private LanguagePair _languageDirection;
		private SdlMTCloudLanguageDirection _languageDirectionProvider;


		public SdlMTCloudTranslationProvider(Uri uri, ConnectionService connectionService, TranslationService translationService, string translationProviderState)
		{
			ConnectionService = connectionService;
			TranslationService = translationService;

			LoadState(translationProviderState);
			Uri = uri;

			var connectionResult = Task.Run(async () => await TranslationService.GetLanguagePairs(ConnectionService.Credential.AccountId)).Result;
			SubscriptionInfo = connectionResult;
		}

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, Constants.PluginName);

		public Uri Uri { get; internal set; }

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

		public Options Options { get; set; }


		public ConnectionService ConnectionService { get; }

		public TranslationService TranslationService { get; }

		public SubscriptionInfo SubscriptionInfo { get; }

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

		public Dictionary<string, string> SupportedLanguages { get; set; }

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			if (_languageDirectionProvider != null &&
				_languageDirectionProvider.SourceLanguage?.Name == languageDirection.SourceCulture.Name &&
				_languageDirectionProvider.TargetLanguage?.Name == languageDirection.TargetCulture.Name)
			{
				return _languageDirectionProvider;
			}

			_languageDirectionProvider = new SdlMTCloudLanguageDirection(this, languageDirection);

			return _languageDirectionProvider;
		}

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			SetSupportedLanguages(_languageDirection);
			return JsonConvert.SerializeObject(Options ?? new Options());
		}

		public void LoadState(string translationProviderState)
		{
			try
			{
				if (string.IsNullOrEmpty(translationProviderState))
				{
					Options = new Options();
					return;
				}

				Options = JsonConvert.DeserializeObject<Options>(translationProviderState);
			}
			catch
			{
				// ignore any casting errors and simply create a new options instance
				Options = new Options();
			}

		}

		private MTCloudLanguagePair SetSupportedLanguages(LanguagePair languageDirection)
		{
			var languageMappings = TranslationService.LanguageMappings;
			if (languageDirection != null && languageMappings != null)
			{
				if (SubscriptionInfo.LanguagePairs?.Count > 0)
				{
					var languagePair = SubscriptionInfo.LanguagePairs
						.FirstOrDefault(o => languageMappings
									   .Any(l => l.SelectedMTCodeSource.CodeName.Equals(o.SourceLanguageId)
												 && l.SelectedMTCodeTarget.CodeName.Equals(o.TargetLanguageId)));

					if (languagePair != null)
					{
						SupportedLanguages = new Dictionary<string, string> { { languageDirection.TargetCulture.Name, languagePair.Name } };
					}

					return languagePair;
				}
			}

			return new MTCloudLanguagePair();
		}
	}
}