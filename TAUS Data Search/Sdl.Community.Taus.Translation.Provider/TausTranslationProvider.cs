using System;
using System.Collections.Generic;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.Taus.Translation.Provider
{
    public class TausTranslationProvider : ITranslationProvider
    {
        ///<summary>
        /// This string needs to be a unique value.
        /// This is the string that precedes the plug-in URI.
        ///</summary>    
        public static readonly string TausTranslationProviderScheme = "taussearch";
        public static readonly string TausApplicationProviderId = "67C52A1B";
        public static Dictionary<string, string> SupportedLanguages;
        public static Dictionary<string, string> AttributesIndustry;
        public static Dictionary<string, string> AttributesContentType;

        static TausTranslationProvider()
        {

            AttributesContentType = new Dictionary<string, string>
            {
                {"0", ""},
                {"1", "Instructions for Use"},
                {"2", "Sales and Marketing Material"},
                {"4", "Policies, Process and Procedures"},
                {"5", "Software Strings and Documentation"},
                {"6", "Undefined Content Type"},
                {"7", "News Announcements, Reports and Research"},
                {"8", "Patents"},
                {"9", "Standards, Statutes and Regulations"},
                {"10", "Financial Documentation"},
                {"12", "Support Content"}
            };

            AttributesIndustry = new Dictionary<string, string>
            {
                {"0", ""},
                {"1", "Automotive Manufacturing"},
                {"2", "Consumer Electronics"},
                {"3", "Computer Software"},
                {"4", "Computer Hardware"},
                {"5", "Industrial Manufacturing"},
                {"6", "Telecommunications"},
                {"7", "Professional and Business Services"},
                {"8", "Stores and Retail Distribution"},
                {"9", "Industrial Electronics"},
                {"10", "Legal Services"},
                {"11", "Energy, Water and Utilities"},
                {"12", "Financials"},
                {"13", "Medical Equipment and Supplies"},
                {"14", "Healthcare"},
                {"15", "Pharmaceuticals and Biotechnology"},
                {"16", "Chemicals"},
                {"17", "Undefined Sector"},
                {"18", "Leisure, Tourism, and Arts"}
            };



            SupportedLanguages = new Dictionary<string, string>
            {
                {"af-ZA", "Afrikaans"},
                {"ar-AE", "Arabic (U.A.E.)"},
                {"ar-AR", "Arabic"},
                {"ar-EG", "Arabic (Egypt)"},
                {"ar-SA", "Arabic (Saudi Arabia)"},
                {"be-BY", "Belarusian"},
                {"bg-BG", "Bulgarian"},
                {"cs-CZ", "Czech"},
                {"cy-GB", "Welsh"},
                {"da-DK", "Danish"},
                {"de-DE", "German (Germany)"},
                {"el-GR", "Greek"},
                {"en-AU", "English (Australia)"},
                {"en-CA", "English (Canada)"},
                {"en-GB", "English (United Kingdom)"},
                {"en-US", "English (United States)"},
                {"en-ZA", "English (South Africa)"},
                {"es-EM", "Spanish (International)"},
                {"es-ES", "Spanish (Spain)"},
                {"es-MX", "Spanish (Mexico)"},
                {"es-XL", "Spanish (Latin America)"},
                {"et-EE", "Estonian"},
                {"eu-ES", "Basque"},
                {"fa-IR", "Farsi"},
                {"fi-FI", "Finnish"},
                {"fr-BE", "French (Belgium)"},
                {"fr-CA", "French (Canada)"},
                {"fr-FR", "French (France)"},
                {"he-IL", "Hebrew (Israel)"},
                {"hr-HR", "Croatian"},
                {"ht-HT", "Haitian"},
                {"hu-HU", "Hungarian"},
                {"id-ID", "Indonesian"},
                {"is-IS", "Icelandic"},
                {"it-IT", "Italian (Italy)"},
                {"ja-JP", "Japanese"},
                {"ko-KR", "Korean"},
                {"lt-LT", "Lithuanian"},
                {"lv-LV", "Latvian"},
                {"mk-MK", "Macedonian"},
                {"ms-MY", "Malay"},
                {"mt-MT", "Maltese"},
                {"nb-NO", "Norwegian (Bokmal)"},
                {"nl-BE", "Dutch (Belgium)"},
                {"nl-NL", "Dutch (Netherlands)"},
                {"nn-NO", "Norwegian (Nynorsk)"},
                {"no-NO", "Norwegian"},
                {"pl-PL", "Polish"},
                {"pt-BR", "Portuguese (Brazil)"},
                {"pt-PT", "Portuguese (Portugal)"},
                {"ro-RO", "Romanian"},
                {"ru-RU", "Russian"},
                {"sk-SK", "Slovak"},
                {"sl-SI", "Slovene"},
                {"sv-SE", "Swedish"},
                {"th-TH", "Thai"},
                {"tr-TR", "Turkish"},
                {"uk-UA", "Ukranian"},
                {"vi-VN", "Vietnamese"},
                {"zh-CN", "Chinese (PRC)"},
                {"zh-HK", "Chinese (Hong Kong)"},
                {"zh-TW", "Chinese (Taiwan)"}
            };
        }

        #region "TausTranslationOptions"
        public TausTranslationOptions Options
        {
            get;
            set;
        }

        public TausTranslationProvider(TausTranslationOptions options)
        {
            Options = options;
        }
        #endregion

        #region "ITranslationProvider Members"
        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new TausTranslationProviderLanguageDirection(this, languageDirection);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void LoadState(string translationProviderState)
        {
            
        }

        public string Name
        {
            get { return PluginResources.Plugin_NiceName; }
        }

        public void RefreshStatusInfo()
        {
            
        }

        public string SerializeState()
        {
            // Save settings
            return null;
        }

        public ProviderStatusInfo StatusInfo
        {
            get { return new ProviderStatusInfo(true, PluginResources.Plugin_NiceName); }
        }

        #region "SupportsConcordanceSearch"
        public bool SupportsConcordanceSearch
        {
            get { return true; }
        }
        #endregion

        public bool SupportsDocumentSearches
        {
            get { return true; }
        }

        public bool SupportsFilters
        {
            get { return true; }
        }

        #region "SupportsFuzzySearch"
        public bool SupportsFuzzySearch
        {
            get { return true; }
        }
        #endregion

        
        /// <summary>
        /// Determines the language direction of the delimited list file by
        /// reading the first line. Based upon this information it is determined
        /// whether the plug-in supports the language pair that was selected by
        /// the user.
        /// </summary>
        #region "SupportsLanguageDirection"
        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            
            var sourceLanguageIsSupported = false;
            var targetLanguageIsSupported = false;
            foreach (var supportedLanguage in SupportedLanguages)
            {
                if (string.Compare(supportedLanguage.Key, languageDirection.SourceCultureName, StringComparison.OrdinalIgnoreCase) == 0)                
                    sourceLanguageIsSupported = true;

                if (string.Compare(supportedLanguage.Key, languageDirection.TargetCultureName, StringComparison.OrdinalIgnoreCase) == 0)
                    targetLanguageIsSupported = true;                           
            }

            return sourceLanguageIsSupported && targetLanguageIsSupported ? true : false;

        }
        #endregion


        #region "SupportsMultipleResults"
        public bool SupportsMultipleResults
        {
            get { return true; }
        }
        #endregion

        #region "SupportsPenalties"
        public bool SupportsPenalties
        {
            get { return true; }
        }
        #endregion

        public bool SupportsPlaceables
        {
            get { return true; }
        }

        public bool SupportsScoring
        {
            get { return true; }
        }

        #region "SupportsSearchForTranslationUnits"
        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }
        #endregion

        #region "SupportsSourceTargetConcordanceSearch"
        public bool SupportsSourceConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return true; }
        }
        #endregion

        public bool SupportsStructureContext
        {
            get { return true; }
        }

        #region "SupportsTaggedInput"
        public bool SupportsTaggedInput
        {
            get { return true; }
        }
        #endregion


        public bool SupportsTranslation
        {
            get { return true; }
        }

        #region "SupportsUpdate"
        public bool SupportsUpdate
        {
            get { return false; }
        }
        #endregion

        public bool SupportsWordCounts
        {
            get { return true; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return TausTranslationOptions.ProviderTranslationMethod; }
        }

        #region "Uri"
        public Uri Uri
        {
            get { return Options.Uri; }
        }
        #endregion

        #endregion

     
    }
}

