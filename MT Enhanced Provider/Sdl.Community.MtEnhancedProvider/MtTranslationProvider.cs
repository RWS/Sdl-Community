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

        public bool IsReadOnly
        {
            get { return true; } //change to true to support updates
        }

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

        public ProviderStatusInfo StatusInfo
        {
            get { return new ProviderStatusInfo(true, PluginResources.Plugin_NiceName); }
        }

        #region "SupportsConcordanceSearch"
        public bool SupportsConcordanceSearch
        {
            get { return false; }
        }
        #endregion

        public bool SupportsDocumentSearches
        {
            get { return false; }
        }

        public bool SupportsFilters
        {
            get { return false; }
        }

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
                    mstConnect.resetCrd(Options.ClientID, Options.ClientSecret); //reset in case changed since last time the class was constructed
                }

                return mstConnect.isSupportedLangPair(languageDirection.SourceCulture.Name, languageDirection.TargetCulture.Name);
            }
            else if (Options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
            {
                if (gtConnect == null) //instantiate GtApiConnecter if necessary
                {
                    gtConnect = new MtTranslationProviderGTApiConnecter(Options.apiKey);
                }
                else
                {
                    gtConnect.ApiKey = Options.apiKey; //reset in case it has been changed since last time GtApiConnecter was instantiated
                }
                return gtConnect.isSupportedLangPair(languageDirection.SourceCulture, languageDirection.TargetCulture);
            }

            //not likely to get here but...
            return true;
        }
        #endregion

        

        #region "SupportsMultipleResults"
        public bool SupportsMultipleResults
        {
            get { return false; }
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
            get { return false; }
        }

        public bool SupportsScoring
        {
            get { return false; }
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
            get { return false; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return false; }
        }
        #endregion

        public bool SupportsStructureContext
        {
            get { return false; }
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
            get { return false; } //change to true along with readonly to false to enable updating
        }
        #endregion

        public bool SupportsWordCounts
        {
            get { return false; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return MtTranslationOptions.ProviderTranslationMethod; }
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

