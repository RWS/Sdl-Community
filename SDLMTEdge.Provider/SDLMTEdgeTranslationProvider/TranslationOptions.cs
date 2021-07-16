using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Sdl.Community.MTEdge.LPConverter;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.Community.MTEdge.Provider.SDLMTEdgeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider
{
	public class TranslationOptions
	{
		private string ResolvedHost { get; set; }
		
		readonly TranslationProviderUriBuilder _uriBuilder;

		public TranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(TranslationProvider.TranslationProviderScheme);
			UseBasicAuthentication = true;
			Port = 8001;
			LPPreferences = new Dictionary<CultureInfo, SDLMTEdgeLanguagePair>();
		}

		public TranslationOptions(Uri uri) : this()
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

		public Dictionary<CultureInfo, SDLMTEdgeLanguagePair> LPPreferences { get; }

		public bool PersistCredentials { get; set; }

		[JsonIgnore]
		public string ApiToken { get; set; }

		public bool UseBasicAuthentication { get; set; }
		public bool RequiresSecureProtocol { get; set; }	

		#region URI Properties
		public string Host
		{
			get => _uriBuilder.HostName;
			set
			{
				_uriBuilder.HostName = value;
				ResolvedHost = null;
			}
		}

		public int Port
		{
			get => _uriBuilder.Port;
			set => _uriBuilder.Port = value;
		}

		public APIVersion ApiVersion { get; set; }

		public string ApiVersionString => ApiVersion == APIVersion.v1 ? "v1" : "v2";

		public Uri Uri => !string.IsNullOrWhiteSpace(_uriBuilder.HostName) ? _uriBuilder.Uri : null;

		public TradosToMTEdgeLP[] SetPreferredLanguages(LanguagePair[] languagePairs)
		{
			var mtEdgeLanguagePairs = SDLMTEdgeTranslatorHelper.GetLanguagePairs(this);
			if (!mtEdgeLanguagePairs.Any())
			{
				return null;
			}
			var languagePairChoices = languagePairs.GroupJoin(
				mtEdgeLanguagePairs,
				requestedLP =>
					new
					{
						SourceLanguageId = requestedLP.SourceCulture.ToMTEdgeCode(),
						TargetLanguageId = requestedLP.TargetCulture.ToMTEdgeCode()
					},
				installedLP =>
					new
					{
						SourceLanguageId = installedLP.SourceLanguageId,
						TargetLanguageId = installedLP.TargetLanguageId
					},
				(requestedLP, installedLP) =>
					new TradosToMTEdgeLP(
						tradosCulture: requestedLP.TargetCulture,
						mtEdgeLPs: installedLP.OrderBy(lp => lp.LanguagePairId).ToList())
			).ToList();

			var customEnginesMapping = new CustomEngines();

			//Fix for Spanish latin amerincan flavours
			CheckLatinSpanish(languagePairs, languagePairChoices, mtEdgeLanguagePairs);

			// Fix for French Canada engine	 which has language code on server frc
			CheckForFrCanada(languagePairs, languagePairChoices, mtEdgeLanguagePairs);

			//there is no engine which has ptb as source, we need to map it to por engine
			CheckForPtbSource(languagePairs, languagePairChoices, mtEdgeLanguagePairs);

			return languagePairChoices.ToArray();
		}

		/// <summary>
		/// Set dictionaries for each languagePairChoices 
		/// </summary>
		/// <param name="languagePairChoices"></param>
		public void SetDictionaries(TradosToMTEdgeLP[] languagePairChoices)
		{
			foreach (var languagePair in languagePairChoices)
			{
				SDLMTEdgeTranslatorHelper.GetDictionaries(languagePair, this);
			}
		}


		private void CheckForPtbSource(LanguagePair[] languagePairs, List<TradosToMTEdgeLP> languagePairChoices, SDLMTEdgeLanguagePair[] mtEdgeLanguagePairs)
		{
			var customEnginesMapping = new CustomEngines();
			var ptbSource = languagePairs.FirstOrDefault(lp => lp.SourceCulture.ThreeLetterWindowsLanguageName.Equals("PTB"));
			if (ptbSource != null)
			{
				var mtEdgeLangPairEngines = mtEdgeLanguagePairs
					.Where(lp => lp.SourceLanguageId.Equals(customEnginesMapping.PortugueseSourceEngineCode.ToLower())).ToList();
				var projectSourceLanguage =
					languagePairChoices.FirstOrDefault(s =>
						s.TradosCulture.ThreeLetterWindowsLanguageName.Equals(ptbSource.TargetCulture.ThreeLetterWindowsLanguageName));
				foreach (var mtEdgeEngine in mtEdgeLangPairEngines)
				{
					projectSourceLanguage?.MtEdgeLPs?.Add(mtEdgeEngine);
				}
			}
		}

		private void CheckForFrCanada(LanguagePair[] languagePairs, List<TradosToMTEdgeLP> languagePairChoices, SDLMTEdgeLanguagePair[] mtEdgeLanguagePairs)
		{
			var customEnginesMapping = new CustomEngines();

			var frenchCanadianLp = languagePairs.FirstOrDefault(lp =>
				lp.SourceCulture.ThreeLetterWindowsLanguageName.Equals("FRC") ||
				lp.TargetCulture.ThreeLetterWindowsLanguageName.Equals("FRC"));
			if (frenchCanadianLp != null)
			{
				AddAditionalMtEdgeEngine(customEnginesMapping.FrenchCanadaEngineCode, customEnginesMapping.FrenchCanadaEngineCode,
					languagePairChoices, mtEdgeLanguagePairs);
			}
		}

		private void CheckLatinSpanish(LanguagePair[] languagePairs, List<TradosToMTEdgeLP> languagePairChoices, SDLMTEdgeLanguagePair[] mtEdgeLanguagePairs)
		{
			var customEnginesMapping = new CustomEngines();
			foreach (var languagePair in languagePairs)
			{
				var sourceSpanish = customEnginesMapping.LatinAmericanLanguageCodes.FirstOrDefault(s =>
					s.Equals(languagePair.SourceCulture.ThreeLetterWindowsLanguageName));

				if (sourceSpanish != null)
				{
					AddAditionalMtEdgeEngine(languagePair.SourceCulture.ThreeLetterWindowsLanguageName,
						customEnginesMapping.SpanishLatinAmericanEngineCode, languagePairChoices,
						mtEdgeLanguagePairs);
				}
				else
				{
					var targetSpanish = customEnginesMapping.LatinAmericanLanguageCodes.FirstOrDefault(s =>
						s.Equals(languagePair.TargetCulture.ThreeLetterWindowsLanguageName));
					if (targetSpanish != null)
					{
						AddAditionalMtEdgeEngine(languagePair.TargetCulture.ThreeLetterWindowsLanguageName,
							customEnginesMapping.SpanishLatinAmericanEngineCode, languagePairChoices,
							mtEdgeLanguagePairs);
					}
				}
			}
		}


		/// <summary>
		/// Used for flavours of a language to map the flavour to parent language code
		/// </summary>
		private void AddAditionalMtEdgeEngine(string languageWindowsCode, string engineCode,List<TradosToMTEdgeLP> languagePairChoices, SDLMTEdgeLanguagePair[] mtEdgeLanguagePairs)
		{
			if (!string.IsNullOrEmpty(engineCode))
			{
				{
					var mtEdgeLangPairEngines = mtEdgeLanguagePairs.Where(lp => lp.SourceLanguageId.Equals(engineCode.ToLower()) ||
					                                                         lp.TargetLanguageId.Equals(engineCode.ToLower())).ToList();
					var projectSourceLanguage = languagePairChoices.FirstOrDefault(s => s.TradosCulture.ThreeLetterWindowsLanguageName.Equals(languageWindowsCode));
					foreach (var mtEdgeEngine in mtEdgeLangPairEngines)
					{
						projectSourceLanguage?.MtEdgeLPs?.Add(mtEdgeEngine);
					}
				}
			}
		}
		
		#endregion
	}
}