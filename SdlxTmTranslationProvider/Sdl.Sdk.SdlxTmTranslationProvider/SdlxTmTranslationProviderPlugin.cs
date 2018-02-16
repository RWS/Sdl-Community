//-----------------------------------------------------------------------
// <copyright file="SdlxTmTranslationProviderPlugin.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;

namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// The main SDLX translation memory translation provider.
    /// </summary>
    public class SdlxTmTranslationProviderPlugin : ITranslationProvider
    {
        /// <summary>
        /// The factory Id for the SDLX TM plug-in
        /// </summary>
        public const string FactoryId = "SdlxTmTranslationProviderPlugin_FactoryId";

        /// <summary>
        /// The factory name for the SDLX TM plug-in
        /// </summary>
        public const string FactoryName = "SdlxTmTranslationProviderPlugin_FactoryName";

        /// <summary>
        /// The translation memory type (e.g. "S42TM-STD")
        /// </summary>
        private string _translationMemoryType;

        /// <summary>
        /// The translation memory connection string
        /// </summary>
        private string _translationMemoryConnectionString;

        /// <summary>
        /// The translation memory reference in the tm controller
        /// </summary>
        private int _translationMemoryReference = 0;

        /// <summary>
        /// A dictionary of segments counts.
        /// </summary>
        private List<SdlxTmLanguagePair> _languageDirections;

        /// <summary>
        /// Initializes a new instance of the <see cref="SdlxTmTranslationProviderPlugin"/> class.
        /// </summary>
        /// <param name="translationMemoryType">The translation memory type.</param>
        /// <param name="translationMemoryConnectionString">The TM connection string.</param>
        public SdlxTmTranslationProviderPlugin(string translationMemoryType, string translationMemoryConnectionString)
        {
            _translationMemoryType = translationMemoryType;
            _translationMemoryConnectionString = translationMemoryConnectionString;
            CloseTm();
            OpenTm();
            GetLanguageDirections();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SdlxTmTranslationProviderPlugin"/> class.
        /// </summary>
        /// <param name="translationMemoryType">The translation memory type.</param>
        /// <param name="translationMemoryConnectionString">The TM connection string.</param>
        /// <param name="translationMemoryReference">The TM reference in the controller.</param>
        public SdlxTmTranslationProviderPlugin(string translationMemoryType, string translationMemoryConnectionString, int translationMemoryReference)
        {
            _translationMemoryType = translationMemoryType;
            _translationMemoryConnectionString = translationMemoryConnectionString;
            _translationMemoryReference = translationMemoryReference;
            GetLanguageDirections();
        }

        #region ITranslationProvider Members

        /// <summary>
        /// Gets a value indicating whether the translation provider or its underlying storage
        /// are read-only. If this flag is <c>true</c>, no data can be deleted, updated, or
        /// added.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get
            {
                return SdlxTmTranslationProvider.TmController.HasRequiredAccess(_translationMemoryReference, 2) == 0;
            }
        }

        /// <summary>
        /// Gets the user-friendly name of this provider. It is not necessarily unique across the system.
        /// </summary>
        /// <value>We use the connection string as the name.</value>
        public string Name
        {
            get { return _translationMemoryConnectionString; }
        }

        /// <summary>
        /// Gets the status info for the provider.
        /// </summary>
        /// <value>Status information about the provider.</value>
        public ProviderStatusInfo StatusInfo
        {
            get
            {
                return new ProviderStatusInfo(true, "Available");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports concordance
        /// search in the source or the target language. For more detailed information,
        /// see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSourceConcordanceSearch"/>
        /// and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTargetConcordanceSearch"/>.
        /// </summary>
        /// <value>True if the provider supports concordance, false otherwise.</value>
        public bool SupportsConcordanceSearch
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports document searches
        /// and will take document (textual) context into account.
        /// </summary>
        /// <value>True if the provider supports document searches, false otherwise.</value>
        public bool SupportsDocumentSearches
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports filters for
        /// text and attribute fields. If false, filters supplied in the search settings
        /// will be ignored.
        /// </summary>
        /// <value>True if the provider supports filters, false otherwise.</value>
        public bool SupportsFilters
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports fuzzy search.
        /// This value should be <c>false</c> if <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTranslation"/> is <c>false</c>.
        /// </summary>
        /// <value>True if the provider supports fuzzy searches, false otherwise.</value>
        public bool SupportsFuzzySearch
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the engine may return multiple results for a search.
        /// If false, at most one result (and translation proposal) will be computed.
        /// </summary>
        /// <value>True if the provider supports multiple results, false otherwise.</value>
        public bool SupportsMultipleResults
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports penalties
        /// of various types. If false, all penalties supplied in the search settings will be ignored.
        /// </summary>
        /// <value>True if the provider supports penalties, false otherwise.</value>
        public bool SupportsPenalties
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports the detection
        /// and handling of placeables, and will return them as part of the search result.
        /// </summary>
        /// <value>True if the provider supports placeables, false otherwise.</value>
        public bool SupportsPlaceables
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider produces scored results
        /// in the rank between [minScore, 100]. If false, no scores will be computed.
        /// </summary>
        /// <value>True if the provider supports scoring, false otherwise.</value>
        public bool SupportsScoring
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports searching
        /// for bilingual input TUs (i.e. the SearchTranslationUnit
        /// method group). If false, the service only supports searching for simple (monolingual)
        /// segments.
        /// </summary>
        /// <value>True if the provider supports searching for translation units, false otherwise.</value>
        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports concordance
        /// search in the source language.
        /// </summary>
        /// <value>True if the provider supports source concordance, false otherwise.</value>
        public bool SupportsSourceConcordanceSearch
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports a structure
        /// context specification and will consider it for searches. If false, the structure
        /// context will be ignored.
        /// </summary>
        /// <value>True if the provider supports structure context, false otherwise.</value>
        public bool SupportsStructureContext
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the translation provider engine supports
        /// processing of tagged input and will return input tags in the translation proposals.
        /// If false, the engine may drop all tags from search segments, and may not return tags
        /// in the output.
        /// </summary>
        /// <value>True if the provider supports tagged input, false otherwise.</value>
        public bool SupportsTaggedInput
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports concordance
        /// search in the target language.
        /// </summary>
        /// <value>True if the provider supports target concordance search, false otherwise.</value>
        public bool SupportsTargetConcordanceSearch
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports translation
        /// search (exact and/or fuzzy search) and the generation of translation proposals. In most
        /// cases this flag will be <c>true</c>. If this
        /// value is <c>false</c>, the translation provider can not be used to request translations,
        /// but can still be used to search for concordance matches, if <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsConcordanceSearch"/>
        /// is <c>true</c>.
        /// </summary>
        /// <value>True if the provider supports translation search, false otherwise.</value>
        public bool SupportsTranslation
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider supports updating/adding
        /// of translation units. If false, all update/add methods will return without effect. Note
        /// that implementations should not throw exceptions for unsupported methods.
        /// </summary>
        /// <value>True if the provider supports updating translation units, false otherwise.</value>
        public bool SupportsUpdate
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the translation provider will compute and return
        /// word counts in searches.
        /// </summary>
        /// <value>True if the provider supports word counting, false otherwise.</value>
        public bool SupportsWordCounts
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the primary method how the translation provider generates translations.
        /// </summary>
        /// <value>The translation method for this provider.</value>
        public TranslationMethod TranslationMethod
        {
            get { return TranslationMethod.TranslationMemory; }
        }

        /// <summary>
        /// Gets a URI which uniquely identifies this translation provider.
        /// </summary>
        /// <value>A unique URI for this translation provider.</value>
        public Uri Uri
        {
            get { return new Uri(string.Format("{0}://{1}/{2}", SdlxTmTranslationProvider.SdlxTmUriScheme, _translationMemoryType, _translationMemoryConnectionString.Replace(@"\", @"/"))); }
        }

        /// <summary>
        /// Gets a translation provider for the specified language direction.
        /// </summary>
        /// <param name="languageDirection">The language direction.</param>
        /// <returns>
        /// The language direction matching the given source and target language.
        /// </returns>
        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            SdlxTmLanguagePair matchingLanguageDirection = GetCompatibleLanguageDirection(languageDirection);

            return (matchingLanguageDirection != null)
                       ? new SdlxTmTranslationProviderLanguageDirection(this, _translationMemoryReference,
                                                                        matchingLanguageDirection.SourceLanguage,
                                                                        matchingLanguageDirection.TargetLanguage) : null;

        }

        /// <summary>
        /// Loads previously serialized state information into this translation provider instance.
        /// </summary>
        /// <param name="translationProviderState">A string representing the state of translation provider that was previously saved
        /// using <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SerializeState"/>.</param>
        /// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
        public void LoadState(string translationProviderState)
        {
            return;
        }

        /// <summary>
        /// Ensures that the provider's status information (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.StatusInfo"/>) is refreshed,
        /// in case it is cached.
        /// </summary>
        public void RefreshStatusInfo()
        {
            return;
        }

        /// <summary>
        /// Serializes any meaningful state information for this translation provider that can be stored in projects
        /// and sent around the supply chain.
        /// </summary>
        /// <returns>
        /// A string representing the state of this translation provider that can later be passed into
        /// the <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.LoadState(System.String)"/> method to restore the state after creating a new translation provider.
        /// </returns>
        /// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
        public string SerializeState()
        {
            return string.Empty;
        }

        /// <summary>
        /// Checks whether this translation provider supports the specified language direction.
        /// </summary>
        /// <param name="languageDirection">The language direction.</param>
        /// <returns>
        /// True if the specified language direction is supported.
        /// </returns>
        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return (GetCompatibleLanguageDirection(languageDirection) != null);
           
        }

        private SdlxTmLanguagePair GetCompatibleLanguageDirection(LanguagePair languageDirection)
        {
            SdlxTmLanguagePair bestMatch = null;

            foreach (SdlxTmLanguagePair lp in _languageDirections)
            {
                // Convert the SDLX language codes to Studio compatible language codes
                CultureInfo sourceCulture = SdlxTmLanguageMap.GetCultureInfoFromIsoCode(lp.SourceLanguage);
                CultureInfo targetCulture = SdlxTmLanguageMap.GetCultureInfoFromIsoCode(lp.TargetLanguage);

                if (sourceCulture == null || targetCulture == null)
                {
                    continue;
                }

                if (Equals(sourceCulture, languageDirection.SourceCulture)
                    && Equals(targetCulture, languageDirection.TargetCulture))
                {
                    bestMatch = lp;
                    break;
                }

                if (bestMatch == null
                    && CultureInfoExtensions.AreCompatible(sourceCulture, languageDirection.SourceCulture)
                    && CultureInfoExtensions.AreCompatible(targetCulture, languageDirection.TargetCulture))
                {
                    bestMatch = lp;
                }
            }

            return bestMatch;
        }

        #endregion

        /// <summary>
        /// Closes the currently open TM, if any.
        /// </summary>
        private void CloseTm()
        {
            try
            {
                if (SdlxTmTranslationProvider.TmController.IsOpen(_translationMemoryReference) != 0)
                {
                    SdlxTmTranslationProvider.TmController.Close(_translationMemoryReference);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Opens the translation memory.
        /// </summary>
        private void OpenTm()
        {
            int sdlxResult = 0;
            try
            {
                sdlxResult = SdlxTmTranslationProvider.TmController.Open(ref _translationMemoryConnectionString, ref _translationMemoryType, 0x12, ref _translationMemoryReference);
            }
            catch
            {
            }

            if (sdlxResult != 0)
            {
                string errorString = string.Empty;
                int originalError = sdlxResult;
                SdlxTmTranslationProvider.TmController.GetLastError(out errorString, ref sdlxResult);
                throw new Exception(string.Format("Error opening TM [{0}/{1}:{2}] ({3}) : {4}", _translationMemoryType, _translationMemoryConnectionString, _translationMemoryReference, originalError, errorString));
            }
        }

        /// <summary>
        /// Gets the language directions for the translation memory.
        /// </summary>
        private void GetLanguageDirections()
        {
            _languageDirections = new List<SdlxTmLanguagePair>();
            List<SdlxTmLanguagePair> listLangPairs = new List<SdlxTmLanguagePair>();
            string infoStr = string.Empty;
            int sdlxResult = 0;

            sdlxResult = SdlxTmTranslationProvider.TmController.GetInfo(_translationMemoryReference, 0x130, out infoStr);
            if (sdlxResult != 0)
            {
                string errorString = string.Empty;
                SdlxTmTranslationProvider.TmController.GetLastError(out errorString, ref sdlxResult);
                throw new Exception(errorString);
            }

            XmlDocument infoXml = new XmlDocument();
            infoStr = infoStr.Substring(infoStr.IndexOf("<"));
            infoXml.LoadXml(infoStr);

            List<KeyValuePair<string, string>> directionalIndexes = new List<KeyValuePair<string, string>>();
            bool useDirectionalIndexes = false;

            try
            {
                XmlNodeList xmlIndexDirections = infoXml.SelectNodes("//indexdirection");
                if (xmlIndexDirections != null && xmlIndexDirections.Count > 0)
                {
                    foreach (XmlNode xmlIndexDirection in xmlIndexDirections)
                    {
                        string src = xmlIndexDirection.Attributes["source"].Value;
                        string tgt = xmlIndexDirection.Attributes["target"].Value;

                        directionalIndexes.Add(new KeyValuePair<string, string>(src, tgt));
                    }

                    useDirectionalIndexes = true;
                }
            }
            catch
            {
            }

            XmlNodeList xmlLangPairs = infoXml.SelectNodes("//languagepair");
            foreach (XmlNode xmlLangPair in xmlLangPairs)
            {
                string src = xmlLangPair.Attributes["source"].Value;
                string tgt = xmlLangPair.Attributes["target"].Value;
                int count = int.Parse(xmlLangPair.Attributes["count"].Value);

                if (useDirectionalIndexes)
                {
                    SdlxTmLanguagePair forwardPair = new SdlxTmLanguagePair(src, tgt);
                    if (directionalIndexes.Contains(new KeyValuePair<string, string>(SdlxTmLanguageMap.GetLcidFromIsoCode(src).ToString(), SdlxTmLanguageMap.GetLcidFromIsoCode(tgt).ToString())))
                    {
                        listLangPairs.Add(forwardPair);
                        _languageDirections.Add(forwardPair);
                    }

                    SdlxTmLanguagePair reversePair = new SdlxTmLanguagePair(tgt, src);
                    if (directionalIndexes.Contains(new KeyValuePair<string, string>(SdlxTmLanguageMap.GetLcidFromIsoCode(tgt).ToString(), SdlxTmLanguageMap.GetLcidFromIsoCode(src).ToString())))
                    {
                        listLangPairs.Add(reversePair);
                        _languageDirections.Add(reversePair);
                    }
                }
                else
                {
                    sdlxResult = SdlxTmTranslationProvider.TmController.IsLanguageIndexed(_translationMemoryReference, ref src);

                    if (sdlxResult != 0)
                    {
                        try
                        {
                            SdlxTmLanguagePair lp = new SdlxTmLanguagePair(src, tgt);
                            listLangPairs.Add(lp);
                            _languageDirections.Add(lp);
                        }
                        catch
                        {
                        }
                    }

                    sdlxResult = SdlxTmTranslationProvider.TmController.IsLanguageIndexed(_translationMemoryReference, ref tgt);

                    if (sdlxResult != 0)
                    {
                        try
                        {
                            SdlxTmLanguagePair lp = new SdlxTmLanguagePair(tgt, src);
                            listLangPairs.Add(lp);
                            _languageDirections.Add(lp);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}
