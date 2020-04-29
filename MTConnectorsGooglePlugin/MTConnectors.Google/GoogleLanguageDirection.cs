using Sdl.LanguagePlatform.MTConnectors.Google.GoogleService;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.MTConnectors.Google.DataContracts
{
    internal class GoogleLanguageDirection : TranslationMemoryApi.AbstractMachineTranslationProviderLanguageDirection
    {
        private GoogleTranslationService _service;
        private readonly object _serviceLockObject = new object();

        public GoogleLanguageDirection(
            TranslationMemoryApi.AbstractMachineTranslationProvider owner,
            Settings settings,
            Core.LanguagePair languageDirection)
            : base(owner, languageDirection)
        {
            var queryRequestBuilder = new QueryRequestBuilder(settings);
            _service = new GoogleTranslationService(LanguageDirection, queryRequestBuilder);
        }

        private string Name
        {
            get
            {
                return string.Format("Google MT Proxy {0}/{1}", LanguageDirection.SourceCultureName, LanguageDirection.TargetCultureName);
            }
        }

        /// <summary>
        /// <see cref="M:System.Object.Equals(object)"/>
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            GoogleLanguageDirection other = obj as GoogleLanguageDirection;
            if (other == null)
            {
                return false;
            }

            if (!LanguageDirection.Equals(other.LanguageDirection))
            {
                return false;
            }

            return true;    // no settings yet so always true
        }

        /// <summary>
        /// <see cref="object.GetHashCode()"/>
        /// </summary>
        /// <returns>A hash code for this object</returns>
        public override int GetHashCode()
        {
            // The name and language direction can be used to hash the configuration of this object
            return (Name + LanguageDirection.ToString()).GetHashCode();
        }

        public string SerializeState()
        {
            return LanguageDirection.ToString();
        }

        protected override TranslationMemory.SearchResults SearchSingleSegmentInternal(TranslationMemory.SearchSettings settings, Core.Segment segment)
        {
            if (segment == null)
            {
                return null;
            }

            lock (_serviceLockObject)
            {
                Core.Segment translation = _service.Translate(segment);
                return CreateSearchResultFromTranslation(segment, translation);
            }
        }

        protected override IList<TranslationMemory.SearchResults> SearchMultipleSegmentsInternal(TranslationMemory.SearchSettings settings, IList<Core.Segment> searchSegments)
        {
            lock (_serviceLockObject)
            {
                List<Core.Segment> translatedSegments = null;

                translatedSegments = new List<Core.Segment>();

                foreach (Core.Segment segment in searchSegments)
                {
                    Core.Segment translation = _service.Translate(segment);

                    // If We Got Nothing Back, Create A Blank Segment To Prevent Errors Further Along
                    if (translation == null)
                    {
                        translation = new Core.Segment(this.LanguageDirection.TargetCulture);
                    }

                    translatedSegments.Add(translation);
                }

                if (translatedSegments.Count != searchSegments.Count)
                {
                    throw new Exception(PluginResources.EMSG_InvalidTranslationCount);
                }

                return CreateSearchResultsFromTranslations(searchSegments, translatedSegments);
            }
        }
    }
}
