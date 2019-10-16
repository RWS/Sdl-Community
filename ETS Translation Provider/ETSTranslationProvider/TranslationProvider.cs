using System;
using System.Linq;
using ETSLPConverter;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ETSTranslationProvider
{
	public class TranslationProvider : ITranslationProvider
	{
		public const string TranslationProviderScheme = "ets";

		public TranslationOptions Options { get; set; }
		public static readonly Log Log = Log.Instance;

		public TranslationProvider(TranslationOptions options)
		{
			Log.Logger.Trace("");
			Options = options;
		}

		#region ITranslationProvider Properties
		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			Log.Logger.Trace("");
			return new TranslationProviderLanguageDirection(this, languageDirection);
		}

		public bool IsReadOnly => true;

		public void LoadState(string translationProviderState)
		{
			Log.Logger.Trace("");
			Options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
		}

		public string Name => PluginResources.Plugin_NiceName;

		public void RefreshStatusInfo()
		{
			Log.Logger.Trace("");
		}

		public string SerializeState()
		{
			Log.Logger.Trace("");
			return JsonConvert.SerializeObject(Options);
		}

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Plugin_NiceName);

		public bool SupportsConcordanceSearch => false;

		public bool SupportsDocumentSearches => false;

		public bool SupportsFilters => false;

		public bool SupportsFuzzySearch => false;

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			//ETS doesn't have ptb as source language, we need to map it to por
			if (languageDirection.SourceCulture.ToETSCode().Equals("ptb"))
			{
				return ETSApi.ETSTranslatorHelper.GetLanguagePairs(Options).Any(lp =>
					lp.SourceLanguageId == "por" &&
					languageDirection.TargetCulture.ToETSCode().Equals(lp.TargetLanguageId));
			}

			return ETSApi.ETSTranslatorHelper.GetLanguagePairs(Options).Any(lp =>
				languageDirection.SourceCulture.ToETSCode().Equals(lp.SourceLanguageId) &&
				languageDirection.TargetCulture.ToETSCode().Equals(lp.TargetLanguageId));
		}

		public bool SupportsMultipleResults => false;

		public bool SupportsPenalties => false;

		public bool SupportsPlaceables => false;

		public bool SupportsScoring => true;

		public bool SupportsSearchForTranslationUnits => true;

		public bool SupportsSourceConcordanceSearch => false;

		public bool SupportsTargetConcordanceSearch => false;

		public bool SupportsStructureContext => false;

		public bool SupportsTaggedInput => true;

		public bool SupportsTranslation => true;

		public bool SupportsUpdate => false;

		public bool SupportsWordCounts => false;

		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		public Uri Uri => Options.Uri;

		#endregion
	}
}