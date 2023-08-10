using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Interface;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.Model
{
	public class TranslationOptions : ITranslationOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public TranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(Constants.TranslationProviderScheme)
			{
				Port = 8001
			};

			UseBasicAuthentication = true;
			RequiresSecureProtocol = false;
			LanguagePairPreferences ??= new Dictionary<CultureInfo, MTEdgeLanguagePair>();
		}

		public TranslationOptions(Uri uri) : this()
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

		public Dictionary<CultureInfo, MTEdgeLanguagePair> LanguagePairPreferences { get; set; }

		[JsonIgnore]
		public List<TradosToMTEdgeLanguagePair> LanguageMapping { get; set; }

		public int Port
		{
			get => _uriBuilder.Port;
			set => _uriBuilder.Port = value;
		}

		public string Host
		{
			get => _uriBuilder.HostName;
			set
			{
				_uriBuilder.HostName = value;
			}
		}

		[JsonIgnore]
		public string ApiToken { get; set; }

		public APIVersion ApiVersion { get; set; }

		public string ApiVersionString => ApiVersion == APIVersion.v1 ? "v1" : "v2";

		public bool PersistCredentials { get; set; }

		public bool UseBasicAuthentication { get; set; }

		public bool UseApiKey { get; set; }

		public bool UseAuth0SSO { get; set; }

		public bool RequiresSecureProtocol { get; set; }

		public Uri Uri => !string.IsNullOrWhiteSpace(_uriBuilder.HostName) ? _uriBuilder.Uri : null;

		public void SetLanguageMapping(List<TradosToMTEdgeLanguagePair> languagePairChoices)
		{
			if (languagePairChoices is null || !languagePairChoices.Any())
			{
				return;
			}

			foreach (var languagePair in languagePairChoices)
			{
				if (languagePair.IsSupported)
				{
					LanguagePairPreferences[languagePair.TradosCulture] = languagePair.SelectedModel;
					LanguagePairPreferences[languagePair.TradosCulture].DictionaryId = languagePair.SelectedDictionary.DictionaryId;
				}
			}
		}

		public void SetDictionaries(TradosToMTEdgeLanguagePair[] languagePairChoices)
		{
			foreach (var languagePair in languagePairChoices.Where(languagePair => languagePair.IsSupported))
			{
				SDLMTEdgeTranslatorHelper.GetDictionaries(languagePair, this);
			}
		}

		public TradosToMTEdgeLanguagePair[] SetPreferredLanguages(LanguagePair[] languagePairs) 
		{
			var mtEdgeLanguagePairs = SDLMTEdgeTranslatorHelper.GetLanguagePairs(this);
			if (!mtEdgeLanguagePairs.Any())
			{
				return null;
			}

			var languagePairChoices = languagePairs.GroupJoin
				(mtEdgeLanguagePairs,
				 requestedLP => new
				 {
					 SourceLanguageId = LanguageRegistryApi.Instance.GetLanguage(requestedLP.SourceCulture).CultureInfo.ToMTEdgeCode(),
					 TargetLanguageId = LanguageRegistryApi.Instance.GetLanguage(requestedLP.TargetCulture).CultureInfo.ToMTEdgeCode()
				 },
				 installedLP => new
				 {
					 installedLP.SourceLanguageId,
					 installedLP.TargetLanguageId
				 },
				 (requestedLP, installedLP) => new TradosToMTEdgeLanguagePair(requestedLP.ToString(), requestedLP.TargetCulture, installedLP.OrderBy(lp => lp.LanguagePairId).ToList()))
				.ToList();

			//Fix for Spanish latin amerincan flavours
			CheckLatinSpanish(languagePairs, languagePairChoices, mtEdgeLanguagePairs);
			//there is no engine which has ptb as source, we need to map it to por engine
			CheckForPtbSource(languagePairs, languagePairChoices, mtEdgeLanguagePairs);
			foreach (var languagePair in languagePairChoices)
			{
				if (languagePair.MtEdgeLanguagePairs is null
				|| !languagePair.MtEdgeLanguagePairs.Any())
				{
					languagePair.IsSupported = false;
				}
			}


			return languagePairChoices.ToArray();
		}

		private void CheckForPtbSource(LanguagePair[] languagePairs, List<TradosToMTEdgeLanguagePair> languagePairChoices, MTEdgeLanguagePair[] mtEdgeLanguagePairs)
        {
            if (languagePairs.FirstOrDefault(lp => LanguageRegistryApi.Instance.GetLanguage(lp.SourceCulture).CultureInfo.ThreeLetterWindowsLanguageName.Equals("PTB")) is not LanguagePair ptbSource)
            {
                return;
            }

            var mtEdgeLangPairEngines = mtEdgeLanguagePairs.Where(lp => lp.SourceLanguageId.Equals(new CustomEngines().PortugueseSourceEngineCode.ToLower())).ToList();
            languagePairChoices.FirstOrDefault(s => s.TradosCulture.ThreeLetterWindowsLanguageName.Equals(LanguageRegistryApi.Instance.GetLanguage(ptbSource.TargetCulture).CultureInfo.ThreeLetterWindowsLanguageName))?
							   .MtEdgeLanguagePairs?
							   .AddRange(mtEdgeLangPairEngines);
        }

        private void CheckForFrCanada(LanguagePair[] languagePairs, List<TradosToMTEdgeLanguagePair> languagePairChoices, MTEdgeLanguagePair[] mtEdgeLanguagePairs)
		{
            if (languagePairs.FirstOrDefault(lp => LanguageRegistryApi.Instance.GetLanguage(lp.SourceCulture).CultureInfo.ThreeLetterWindowsLanguageName.Equals("FRC")
                                                || LanguageRegistryApi.Instance.GetLanguage(lp.TargetCulture).CultureInfo.ThreeLetterWindowsLanguageName.Equals("FRC"))
				is not null)
			{
				var customEnginesMapping = new CustomEngines();
				AddAditionalMtEdgeEngine(customEnginesMapping.FrenchCanadaEngineCode, customEnginesMapping.FrenchCanadaEngineCode, languagePairChoices, mtEdgeLanguagePairs);
			}
		}

		private void CheckLatinSpanish(LanguagePair[] languagePairs, List<TradosToMTEdgeLanguagePair> languagePairChoices, MTEdgeLanguagePair[] mtEdgeLanguagePairs)
		{
			var customEnginesMapping = new CustomEngines();
			foreach (var languagePair in languagePairs)
			{
				var languageCode = string.Empty;
				if (customEnginesMapping.LatinAmericanLanguageCodes.FirstOrDefault(x => x.Equals(LanguageRegistryApi.Instance.GetLanguage(languagePair.SourceCulture).CultureInfo.ThreeLetterWindowsLanguageName))
					is string sourceSpanish)
				{
					languageCode = LanguageRegistryApi.Instance.GetLanguage(languagePair.SourceCulture).CultureInfo.ThreeLetterWindowsLanguageName;
				}
				else if (customEnginesMapping.LatinAmericanLanguageCodes.FirstOrDefault(x => x.Equals(LanguageRegistryApi.Instance.GetLanguage(languagePair.TargetCulture).CultureInfo.ThreeLetterWindowsLanguageName))
					is string targetSpanish)
				{
					languageCode = LanguageRegistryApi.Instance.GetLanguage(languagePair.TargetCulture).CultureInfo.ThreeLetterWindowsLanguageName;
				}

                if (string.IsNullOrEmpty(languageCode))
				{
                    AddAditionalMtEdgeEngine(
						languageCode,
						customEnginesMapping.SpanishLatinAmericanEngineCode,
						languagePairChoices,
						mtEdgeLanguagePairs);
                }
            }
		}

		/// <summary>
		/// Used for flavours of a language to map the flavour to parent language code
		/// </summary>
		private void AddAditionalMtEdgeEngine(string languageWindowsCode, string engineCode, List<TradosToMTEdgeLanguagePair> languagePairChoices, MTEdgeLanguagePair[] mtEdgeLanguagePairs)
		{
			if (string.IsNullOrEmpty(engineCode))
			{
				return;
			}

			var mtEdgeLangPairEngines = mtEdgeLanguagePairs.Where(lp => lp.SourceLanguageId.Equals(engineCode.ToLower())
																	 || lp.TargetLanguageId.Equals(engineCode.ToLower()));
			languagePairChoices.FirstOrDefault(s => s.TradosCulture.ThreeLetterWindowsLanguageName.Equals(languageWindowsCode))?
							   .MtEdgeLanguagePairs?
							   .AddRange(mtEdgeLangPairEngines);
		}
	}
}