using System;
using System.Globalization;
using System.Linq;
using NLog;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.XliffConverter.Converter;
using Sdl.Community.MTEdge.Provider.XliffConverter.Models;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;

namespace Sdl.Community.MTEdge.Provider.Studio
{
	public class ProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly Provider _translationProvider;
		private readonly LanguagePair _languagePair;

		public ProviderLanguageDirection(Provider provider, LanguagePair languages)
		{
			 
			_languagePair = languages;
			_translationProvider = provider;
		}

        public bool CanReverseLanguageDirection => false;

        public CultureInfo SourceLanguage => _languagePair.SourceCulture;

        public CultureInfo TargetLanguage => _languagePair.TargetCulture;

        public ITranslationProvider TranslationProvider => _translationProvider;

		CultureCode ITranslationProviderLanguageDirection.SourceLanguage => _languagePair.SourceCulture;

		CultureCode ITranslationProviderLanguageDirection.TargetLanguage => _languagePair.TargetCulture;

		/// <summary>
		/// Used to translate a single segment.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="segment"></param>
		/// <returns></returns>
		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			 

			if (TranslateSegments(new Segment[] { segment }).FirstOrDefault()
				is not Segment translation)
            {
                return new SearchResults();
            }

            var results = new SearchResults
            {
                SourceSegment = segment.Duplicate()
            };

            results.Add(CreateSearchResult(segment, translation));
			return results;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			// Need this vs having mask parameter default to null as inheritence doesn't allow default values to
			// count as the same thing as having no parameter at all. IE, you can't have
			// public string foo(string s = null) override public string foo().
			return SearchSegments(settings, segments, null);
		}

		/// <summary>
		/// Translate an array of segments.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="segments">Array of segments to be translated (depending on the truthiness of
		/// corresponding mask)</param>
		/// <param name="mask">Whether to translate a segment or not</param>
		/// <returns></returns>
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			 
			var translations = TranslateSegments(segments.Where((seg, i) => mask is null || mask[i]).ToArray());
			if (translations.All(translation => translation is null))
			{
				return new SearchResults[segments.Length];
			}

			var results = new SearchResults[segments.Length];
			var translationIndex = 0;
			for (var i = 0; i < segments.Length; i++)
			{
				if (mask is not null && !mask[i])
				{
					results[i] = null;
					continue;
				}

				results[i] = new SearchResults();
				if (segments[i] is null)
				{
					results[i].SourceSegment = new Segment();
					results[i].Add(CreateSearchResult(new Segment(), new Segment()));
					continue;
				}

				results[i].SourceSegment = segments[i].Duplicate();
				results[i].Add(CreateSearchResult(segments[i], translations[translationIndex]));
				translationIndex++;
			}

			return results;
		}

		/// <summary>
		/// Translate an array of segments depending on the partner mask array's values.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="segments"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			 
			if (segments is null)
			{
				throw new ArgumentNullException("segments", "Segments in SearchSegmentsMasked");
			}

			if (mask is null || mask.Length != segments.Length)
			{
				throw new ArgumentException("Mask in SearchSegmentsMasked");
			}

			return SearchSegments(settings, segments, mask);
		}

		/// <summary>
		/// Translate a string of text.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="segment"></param>
		/// <returns></returns>
		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			 
			var sourceSegment = new Segment(_languagePair.SourceCulture);
			sourceSegment.Add(segment);
			return SearchSegment(settings, sourceSegment);
		}

		/// <summary>
		/// Translate a translation unit using the source segment.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="translationUnit"></param>
		/// <returns></returns>
		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			 
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		/// <summary>
		/// Translate an array of translation units using their source segments.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="translationUnits"></param>
		/// <returns></returns>
		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			 
			return SearchSegments(settings, translationUnits.Select(tu => tu.SourceSegment).ToArray());
		}

		/// <summary>
		/// Translate an array of translation units using their source segments.
		/// Translation depends on the corresponding mask array.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="translationUnits"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			 
			if (translationUnits is null)
			{
				throw new ArgumentNullException("translationUnits", "TranslationUnits in SearchSegmentsMasked");
			}

			if (mask is null || mask.Length != translationUnits.Length)
			{
				throw new ArgumentException("Mask in SearchSegmentsMasked");
			}

			return SearchSegments(settings, translationUnits.Select(tu => tu?.SourceSegment).ToArray(), mask);
		}

        /// <summary>
        /// Creates a translated segment by calling upon the MTEdge API (via helper methods)
        /// </summary>
        /// <param name="sourceSegment"></param>
        /// <returns></returns>
        private Segment[] TranslateSegments(Segment[] sourceSegments)
        {
            var xliffDocument = CreateXliffFile(sourceSegments);
            var translatedXliffText = SDLMTEdgeTranslatorHelper.GetTranslation(_translationProvider.Options, _languagePair, xliffDocument);
            return Converter.ParseXliffString(translatedXliffText) is Xliff translatedXliff
                 ? translatedXliff.GetTargetSegments()
                 : (new Segment[sourceSegments.Length]);
        }

        /// <summary>
        /// Prepares a source translation string for request. It ultimately removes all the tags data,
        /// storing them temporarily in a dictionary, which then can be used to reinstate the tags' text.
        /// Function taken from previous MTEdge Plugin.
        /// </summary>
        /// <param name="segments"></param>
        /// <returns>Xliff</returns>
        private Xliff CreateXliffFile(Segment[] segments)
        {
            var file = new File
            {
                SourceCulture = _languagePair.SourceCulture,
                TargetCulture = _languagePair.TargetCulture
            };

            var xliffDocument = new Xliff
            {
                File = file
            };

            foreach (var segment in segments.Where(seg => seg is not null))
            {
                xliffDocument.AddSourceSegment(segment);
            }

            return xliffDocument;
        }

        /// <summary>
        /// Creates a consumable SearchResult using the source segment and translated segment
        /// </summary>
        /// <param name="searchSegment"></param>
        /// <param name="translation"></param>
        /// <param name="sourceSegment"></param>
        /// <returns></returns>
        private SearchResult CreateSearchResult(Segment searchSegment, Segment translation)
        {
             
            var unit = new TranslationUnit
            {
                SourceSegment = searchSegment,
                TargetSegment = translation,
                ConfirmationLevel = ConfirmationLevel.Translated,
                Origin = TranslationUnitOrigin.Nmt
            };
            unit.ResourceId = new PersistentObjectToken(unit.GetHashCode(), Guid.Empty);

            return new SearchResult(unit)
            {
                // We do not currently support scoring, so always say that we're 25% sure on this translation.
                ScoringResult = new ScoringResult() { BaseScore = 25 }
            };
        }

        #region Unnecessary Training Methods
        /// <summary>
        /// Not required for this implementation. This is used to "train" the source, which if it's readonly
        /// doesn't work.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="settings"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			 
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation. This is used to "train" the source, which if it's readonly
		/// doesn't work.
		/// </summary>
		/// <param name="translationUnit"></param>
		/// <returns></returns>
		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			 
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation. This is used to "train" the source, which if it's readonly
		/// doesn't work.
		/// </summary>
		/// <param name="translationUnits"></param>
		/// <returns></returns>
		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			 
			throw new NotImplementedException();
		}
		/// <summary>
		/// Not required for this implementation. This is used to "train" the source, which if it's readonly
		/// doesn't work.
		/// </summary>
		/// <param name="translationUnits"></param>
		/// <param name="previousTranslationHashes"></param>
		/// <param name="settings"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			 
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation. This is used to "train" the source, which if it's readonly
		///  doesn't work.
		/// </summary>
		/// <param name="translationUnit"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			 
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation. This is used to "train" the source, which if it's readonly
		/// doesn't work.
		/// </summary>
		/// <param name="translationUnits"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			 
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation. This is used to "train" the source, which if it's readonly
		/// doesn't work.
		/// </summary>
		/// <param name="translationUnits"></param>
		/// <param name="previousTranslationHashes"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			 
			throw new NotImplementedException();
		}
		#endregion
	}
}