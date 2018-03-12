// ---------------------------------
// <copyright file="AbstractTrados2007TranslationProvider.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------

using Sdl.Community.Trados2007;
using Trados.Interop.TMAccess;

namespace Sdl.Community.Trados2007
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using System.Xml.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors",
    Justification = "By original SDL API design.")]
    public abstract class AbstractTrados2007TranslationProvider : ITrados2007TranslationProvider
    {
        //protected TranslationMemoryClass tradosProvider;
        /// <summary>
        /// Trados 2007 Translation Provider Uri Scheme
        /// </summary>
        public static readonly string UriScheme = "ttp";

        protected static TP2007Pull providersPull = new TP2007Pull();

        /// <summary>
        /// Provider settings
        /// </summary>
        private readonly TranslationProviderSettings settings;


        public abstract TranslationMemoryClass TradosProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTrados2007TranslationProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected AbstractTrados2007TranslationProvider(TranslationProviderSettings settings)
        {
            this.settings = settings;
        }

        #region ITrados2007TranslationProvider Properties

        /// <summary>
        /// Gets the supported language direction.
        /// </summary>
        public abstract LanguagePair LanguageDirection { get; }

        #endregion // ITrados2007TranslationProvider

        #region ITranslationProvider Properties

        /// <summary>
        /// Gets the status info for the provider.
        /// </summary>
        public ProviderStatusInfo StatusInfo
        {
            get
            {
                return new ProviderStatusInfo(true, PluginResources.Trados2007_StatusMessage);
            }
        }

        /// <summary>
        /// Gets a URI which uniquely identifies this translation provider.
        /// </summary>
        public virtual Uri Uri
        {
            get
            {
                return this.Settings.Uri;
            }
        }

        /// <summary>
        /// Gets the user-friendly name of this provider. It is not necessarily unique across the system.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.Settings.TranslationMemoryName;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether or not the translation provider engine supports 
        ///             processing of tagged input and will return input tags in the translation proposals. 
        ///             If false, the engine may drop all tags from search segments, and may not return tags
        ///             in the output.
        /// </summary>
        public bool SupportsTaggedInput
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider produces scored results 
        ///             in the rank between [minScore, 100]. If false, no scores will be computed.
        /// </summary>
        public bool SupportsScoring
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports searching 
        ///             for bilingual input TUs (i.e. the SearchTranslationUnit
        ///             method group). If false, the service only supports searching for simple (monolingual) 
        ///             segments.
        /// </summary>
        public bool SupportsSearchForTranslationUnits
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the engine may return multiple results for a search. 
        ///             If false, at most one result (and translation proposal) will be computed.
        /// </summary>
        public bool SupportsMultipleResults
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports filters for 
        ///             text and attribute fields. If false, filters supplied in the search settings 
        ///             will be ignored.
        /// </summary>
        public bool SupportsFilters
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports penalties 
        ///             of various types. If false, all penalties supplied in the search settings will be ignored.
        /// </summary>
        public bool SupportsPenalties
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports a structure 
        ///             context specification and will consider it for searches. If false, the structure 
        ///             context will be ignored.
        /// </summary>
        public bool SupportsStructureContext
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports document searches 
        ///             and will take document (textual) context into account.
        /// </summary>
        public bool SupportsDocumentSearches
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports updating/adding 
        ///             of translation units. If false, all update/add methods will return without effect. Note 
        ///             that implementations should not throw exceptions for unsupported methods.
        /// </summary>
        public bool SupportsUpdate
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports the detection 
        ///             and handling of placeables, and will return them as part of the search result.
        /// </summary>
        public bool SupportsPlaceables
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports translation 
        ///             search (exact and/or fuzzy search) and the generation of translation proposals. In most 
        ///             cases this flag will be <c>true</c>. If this 
        ///             value is <c>false</c>, the translation provider can not be used to request translations, 
        ///             but can still be used to search for concordance matches, if <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsConcordanceSearch"/>
        ///             is <c>true</c>. 
        /// </summary>
        public bool SupportsTranslation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports fuzzy search. 
        ///             This value should be <c>false</c> if <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTranslation"/> is <c>false</c>.
        /// </summary>
        public bool SupportsFuzzySearch
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports concordance 
        ///             search in the source or the target language. For more detailed information, 
        ///             see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSourceConcordanceSearch"/> 
        ///             and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTargetConcordanceSearch"/>.
        /// </summary>
        public bool SupportsConcordanceSearch
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports concordance 
        ///             search in the source language.
        /// </summary>
        public bool SupportsSourceConcordanceSearch
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports concordance 
        /// search in the target language. Trados 2007 does not support target concordance search.
        /// </summary>
        public bool SupportsTargetConcordanceSearch
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider will compute and return 
        ///             word counts in searches.
        /// </summary>
        public bool SupportsWordCounts
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider or its underlying storage
        /// are read-only. If this flag is <c>true</c>, no data can be deleted, updated, or
        /// added.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the primary method how the translation provider generates translations.
        /// </summary>
        public TranslationMethod TranslationMethod
        {
            get
            {
                return TranslationMethod.TranslationMemory;
            }
        }

        /// <summary>
        /// Gets the provider settings.
        /// </summary>
        protected TranslationProviderSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        #endregion // Properties

        #region Methods static

        /// <summary>
        /// TODO!
        /// Gets the translation provider name from settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>Provider's friendly name.</returns>
        //public static string GetTranslationProviderNameFromSettings(TranslationProviderSettings settings)
        //{
        //    string name = string.Format(
        //        "SDL Trados 2007 {0}", 
        //        settings.TranslationMemoryName);

        //    return name;
        //}

        #endregion // Methods static

        #region Methods

        /// <summary>
        /// Checks whether this translation provider supports the specified language direction.
        /// Assumes that Trados 2007 TP supports only one language direction.
        /// </summary>
        /// <param name="languageDirection">The language direction.</param>
        /// <returns>
        /// True if the specified language direction is supported.
        /// </returns>
        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return this.LanguageDirection.IsCompatible(languageDirection);
        }

        /// <summary>
        /// Ensures that the provider's status information (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.StatusInfo"/>) is refreshed,
        /// in case it is cached.
        /// </summary>
        public void RefreshStatusInfo()
        {
            // do nothing
        }

        /// <summary>
        /// Serializes any meaningful state information for this translation provider that can be stored in projects
        /// and sent around the supply chain.
        /// </summary>
        /// <returns>
        /// A string representing the state of this translation provider that can later be passed into
        /// the <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.LoadState(System.String)"/> method to restore the state after creating a new translation provider.
        /// </returns>
        public string SerializeState()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TP2007ProviderSerializationHelper));

            var helper = new TP2007ProviderSerializationHelper(this);
            using (var writer = new System.IO.StringWriter())
            {
                serializer.Serialize(writer, helper);

                return writer.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Loads previously serialized state information into this translation provider instance.
        /// </summary>
        /// <param name="translationProviderState">A string representing the state of translation provider that was previously saved
        /// using <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SerializeState"/>.</param>
        public void LoadState(string translationProviderState)
        {
            // nothing to load
        }

        #endregion // Methods

        #region Methods abstract

        /// <summary>
        /// Gets a translation provider for the specified language direction.
        /// </summary>
        /// <param name="languageDirection">The language direction.</param>
        /// <returns>
        /// The language direction matching the given source and target language.
        /// </returns>
        public abstract ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection);

        #endregion // Methods abstract
    }
}