/* Copyright 2015 Patrick Porter

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/

using System;
using Newtonsoft.Json;
using Sdl.Community.MtEnhancedProvider.GoogleApi;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.MstConnect;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider
{
	public class MtTranslationProvider : ITranslationProvider
	{
		/// <summary>
		///     This string needs to be a unique value.
		///     It is the string that precedes the plug-in URI.
		/// </summary>
		public static readonly string ListTranslationProviderScheme = "mtenhancedprovider";

		private MtTranslationProviderGTApiConnecter _gtConnect;
		private GoogleV3Connecter _googleV3Connecter;
		private ApiConnecter _mstConnect;

		public MtTranslationProvider(IMtTranslationOptions options)
		{
			Options = options;
		}

		public IMtTranslationOptions Options { get; set; }

		public bool IsReadOnly => true;

		public string Name
		{
			get
			{
				if (Options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
				{
					return Options.SelectedGoogleVersion == Enums.GoogleApiVersion.V2 ? PluginResources.GoogleBasic : PluginResources.GoogleAdvanced;
				}
				if (Options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
					return PluginResources.Microsoft_Name;
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

		public TranslationMethod TranslationMethod => MtTranslationOptions.ProviderTranslationMethod;

		public Uri Uri => Options.Uri;

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new MtTranslationProviderLanguageDirection(this, languageDirection);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<MtTranslationOptions>(translationProviderState);
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
			if (Options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
			{
				if (_mstConnect == null) //construct ApiConnecter if necessary 
					_mstConnect = new ApiConnecter(Options.ClientId);
				else
					_mstConnect.ResetCrd(Options.ClientId); //reset in case changed since last time the class was constructed

				return _mstConnect.IsSupportedLangPair(languageDirection.SourceCulture.Name,
					languageDirection.TargetCulture.Name);
			}
			if (Options.SelectedGoogleVersion == Enums.GoogleApiVersion.V2)
			{
				if (_gtConnect == null) //instantiate GtApiConnecter if necessary
					_gtConnect = new MtTranslationProviderGTApiConnecter(Options.ApiKey);
				else
					_gtConnect.ApiKey =
						Options.ApiKey; //reset in case it has been changed since last time GtApiConnecter was instantiated
				return _gtConnect.IsSupportedLangPair(languageDirection.SourceCulture, languageDirection.TargetCulture);
			}
			if (_googleV3Connecter == null)
			{
				_googleV3Connecter = new GoogleV3Connecter(Options);
			}
			else
			{
				_googleV3Connecter.ProjectName = Options.ProjectName;
				_googleV3Connecter.JsonFilePath = Options.JsonFilePath;
				_googleV3Connecter.GlossaryPath = Options.GlossaryPath;
			}

			return _googleV3Connecter.IsSupportedLanguage(languageDirection.SourceCulture, languageDirection.TargetCulture);
		}
	}
}