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
using Sdl.Community.MtEnhancedProvider.MstConnect;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider
{
    public class MtTranslationProvider : ITranslationProvider
    {
        ///<summary>
        /// This string needs to be a unique value.
        /// It is the string that precedes the plug-in URI.
        ///</summary>    
        public static readonly string ListTranslationProviderScheme = "mtenhancedprovider";
        private MtTranslationProviderGTApiConnecter gtConnect;
        private ApiConnecter mstConnect;

        

        #region "ListTranslationOptions"
        public MtTranslationOptions Options
        {
            get;
            set;
        }

        public MtTranslationProvider(MtTranslationOptions options)
        {

            Options = options;

        }
        #endregion

        #region "ITranslationProvider Members"
        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new MtTranslationProviderLanguageDirection(this, languageDirection);
        }

        public bool IsReadOnly => true;

        public void LoadState(string translationProviderState)
        {
        }

        public string Name
        {
            get
            {
                if (Options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
                    return PluginResources.Google_Name;
                else if (Options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
                    return PluginResources.Microsoft_Name;
                else
                    return PluginResources.Plugin_Name;
            }
        }

        public void RefreshStatusInfo()
        {

        }

        public string SerializeState()
        {
            // Save settings
            return null;
        }

        public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Plugin_NiceName);

        #region "SupportsConcordanceSearch"
        public bool SupportsConcordanceSearch { get; } = false;

        #endregion

        public bool SupportsDocumentSearches { get; } = false;

        public bool SupportsFilters { get; } = false;

        #region "SupportsFuzzySearch"
        public bool SupportsFuzzySearch
        {
            get { return false; }
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

            if (Options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
            {
                if (mstConnect == null) //construct ApiConnecter if necessary 
                {
                    mstConnect = new ApiConnecter(Options);
                }
                else
                {
                    mstConnect.resetCrd(Options.ClientId, Options.ClientSecret); //reset in case changed since last time the class was constructed
                }

                return mstConnect.isSupportedLangPair(languageDirection.SourceCulture.Name, languageDirection.TargetCulture.Name);
            }
            else if (Options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
            {
                if (gtConnect == null) //instantiate GtApiConnecter if necessary
                {
                    gtConnect = new MtTranslationProviderGTApiConnecter(Options.ApiKey);
                }
                else
                {
                    gtConnect.ApiKey = Options.ApiKey; //reset in case it has been changed since last time GtApiConnecter was instantiated
                }
                return gtConnect.IsSupportedLangPair(languageDirection.SourceCulture, languageDirection.TargetCulture);
            }

            //not likely to get here but...
            return true;
        }
        #endregion

        

        #region "SupportsMultipleResults"
        public bool SupportsMultipleResults => false;

        #endregion

        #region "SupportsPenalties"
        public bool SupportsPenalties => true;

        #endregion

        public bool SupportsPlaceables => false;

        public bool SupportsScoring => false;

        #region "SupportsSearchForTranslationUnits"
        public bool SupportsSearchForTranslationUnits => true;

        #endregion

        #region "SupportsSourceTargetConcordanceSearch"
        public bool SupportsSourceConcordanceSearch => false;

        public bool SupportsTargetConcordanceSearch => false;

        #endregion

        public bool SupportsStructureContext { get; } = false;

        #region "SupportsTaggedInput"
        public bool SupportsTaggedInput => true;

        #endregion


        public bool SupportsTranslation => true;

        #region "SupportsUpdate"
        public bool SupportsUpdate => false;

        #endregion

        public bool SupportsWordCounts => false;

        public TranslationMethod TranslationMethod => MtTranslationOptions.ProviderTranslationMethod;

        #region "Uri"
        public Uri Uri => Options.Uri;

        #endregion



        #endregion
    }
}

