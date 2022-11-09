using Google.Protobuf.WellKnownTypes;
using GoogleTranslatorProvider.GoogleAPI;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleTranslatorProvider.Studio
{
	public class Provider : ITranslationProvider
	{
		/// <summary>
		///     This string needs to be a unique value.
		///     It is the string that precedes the plug-in URI.
		/// </summary>
		public static readonly string ListTranslationProviderScheme = "mtenhancedprovider";
		private ApiConnecter _gtConnect;
		private GoogleV3Connecter _googleV3Connecter;
		private readonly HtmlUtil _htmlUtil;

		public Provider(ITranslationOptions options, HtmlUtil htmlUtil)
		{
			Options = options;
			_htmlUtil = htmlUtil;
		}

		public ITranslationOptions Options { get; set; }

		public bool IsReadOnly => true;

		public string Name
		{
			get
			{
				if (Options.SelectedProvider == GTPTranslationOptions.ProviderType.GoogleTranslate)
				{
					return Options.SelectedGoogleVersion == ApiVersion.V2 ? PluginResources.GoogleBasic : PluginResources.GoogleAdvanced;
				}

				return PluginResources.Plugin_Name;
			}
		}

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Plugin_NiceName);

		public bool SupportsConcordanceSearch { get; } = false;

		public bool SupportsDocumentSearches { get; } = false;

		public bool SupportsFilters { get; } = false;

		public bool SupportsFuzzySearch => false;

		public bool SupportsMultipleResults => false;

		public bool SupportsPenalties => true;

		public bool SupportsPlaceables => false;

		public bool SupportsScoring => false;

		public bool SupportsSearchForTranslationUnits => true;

		public bool SupportsSourceConcordanceSearch => false;

		public bool SupportsStructureContext { get; } = false;

		public bool SupportsTaggedInput => true;

		public bool SupportsTargetConcordanceSearch => false;

		public bool SupportsTranslation => true;

		public bool SupportsUpdate => false;

		public bool SupportsWordCounts => false;

		public TranslationMethod TranslationMethod => GTPTranslationOptions.ProviderTranslationMethod;

		public Uri Uri => Options.Uri;

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new ProviderLanguageDirection(this, languageDirection, _htmlUtil);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<GTPTranslationOptions>(translationProviderState);
		}

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(Options);
		}

		/// <summary>
		///     Determines the language direction of the delimited list file by
		///     reading the first line. Based upon this information it is determined
		///     whether the plug-in supports the language pair that was selected by
		///     the user.
		/// </summary>
		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{

			if (Options.SelectedGoogleVersion == ApiVersion.V2)
			{
				if (_gtConnect == null) //instantiate GtApiConnecter if necessary
				{
					_gtConnect = new ApiConnecter(Options.ApiKey, _htmlUtil);
				}
				else
				{
					//reset in case it has been changed since last time GtApiConnecter was instantiated
					_gtConnect.ApiKey = Options.ApiKey;
				}

				return _gtConnect.IsSupportedLanguagePair(languageDirection.SourceCulture, languageDirection.TargetCulture);
			}

			_googleV3Connecter = new GoogleV3Connecter(Options);


			return _googleV3Connecter.IsSupportedLanguage(languageDirection.SourceCulture, languageDirection.TargetCulture);
		}
	}
}