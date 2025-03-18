using Newtonsoft.Json;
using Sdl.Community.AmazonTranslateTradosPlugin.Model;
using Sdl.Community.AmazonTranslateTradosPlugin.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Studio.TranslationProvider
{
    public interface ITranslationProviderExtension
    {
        Dictionary<string, string> LanguagesSupported { get; set; }
    }

    public class TranslationProvider : ITranslationProvider, ITranslationProviderExtension
    {
        ///<summary>
        /// This string needs to be a unique value.
        /// It is the string that precedes the plug-in URI.
        ///</summary>
        public static readonly string TranslationProviderScheme = "amazontranslateprovider";

        public TranslationProvider(TranslationOptions options)
        {
            TranslationOptions = options;
        }

        public TranslationOptions TranslationOptions { get; set; }

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new TranslationProviderLanguageDirection(this, languageDirection);
        }

        public bool IsReadOnly => true;

        public void LoadState(string translationProviderState)
        {
            TranslationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
        }

        public string Name => PluginResources.Plugin_Name;

        public void RefreshStatusInfo() { }

        public string SerializeState()
        {
            // Save settings
            return JsonConvert.SerializeObject(TranslationOptions);
        }

        public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Plugin_NiceName);

        public bool SupportsConcordanceSearch => false;

        public bool SupportsDocumentSearches => false;

        public bool SupportsFilters => false;

        public bool SupportsFuzzySearch => false;

        /// <summary>
        /// Determines the language direction of the delimited list file by
        /// reading the first line. Based upon this information it is determined
        /// whether the plug-in supports the language pair that was selected by
        /// the user.
        /// </summary>

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            //for now just...
            return true;
        }

        public bool SupportsMultipleResults => false;

        public bool SupportsPenalties => true;

        public bool SupportsPlaceables => false;

        public bool SupportsScoring => false;

        public bool SupportsSearchForTranslationUnits => true;

        public bool SupportsSourceConcordanceSearch => false;

        public bool SupportsTargetConcordanceSearch => false;

        public bool SupportsStructureContext { get; } = false;

        public bool SupportsTaggedInput => true;

        public bool SupportsTranslation => true;

        public bool SupportsUpdate => false;

        public bool SupportsWordCounts => false;

        public TranslationMethod TranslationMethod => TranslationOptions.ProviderTranslationMethod;

        public Uri Uri => TranslationOptions.Uri;

        public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();
    }
}