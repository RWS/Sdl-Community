using System;
using System.Linq;
using NLog;
using Sdl.Community.MTEdge.Provider.SDLMTEdgeApi;
using Sdl.Community.MTEdge.Provider.XliffConverter.Converter;
using Sdl.Community.MTEdge.Provider.XliffConverter.Models;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;

namespace Sdl.Community.MTEdge.Provider
{
	public class TranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly TranslationProvider provider;
		private readonly LanguagePair languageDirection;

		private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		public TranslationProviderLanguageDirection(TranslationProvider provider, LanguagePair languages)
		{
			_logger.Trace("");
			this.provider = provider;
			languageDirection = languages;
		}

		public System.Globalization.CultureInfo SourceLanguage
		{
			get { return languageDirection.SourceCulture; }
		}

		public System.Globalization.CultureInfo TargetLanguage
		{
			get { return languageDirection.TargetCulture; }
		}

		public ITranslationProvider TranslationProvider
		{
			get { return provider; }
		}

		public bool CanReverseLanguageDirection
		{
			get { return false; }
		}

		/// <summary>
		/// Used to translate a single segment.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="segment"></param>
		/// <returns></returns>
		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			_logger.Trace("");

			var results = new SearchResults();
			var translation = TranslateSegments(new Segment[] { segment }).First();
			if (translation != null)
			{
				results.SourceSegment = segment.Duplicate();
				results.Add(CreateSearchResult(segment, translation));
				return results;
			}
			return new SearchResults();
		}

		/// <summary>
		/// Creates a translated segment by calling upon the MTEdge API (via helper methods)
		/// </summary>
		/// <param name="sourceSegment"></param>
		/// <returns></returns>
		private Segment[] TranslateSegments(Segment[] sourceSegments)
		{
			var xliffDocument = CreateXliffFile(sourceSegments);

			var translatedXliffText = SDLMTEdgeTranslatorHelper.GetTranslation(
				provider.Options,
				languageDirection,
				xliffDocument);

			var translatedXliff = Converter.ParseXliffString(translatedXliffText);
			if (translatedXliff != null)
			{
				return translatedXliff.GetTargetSegments(out _);
			}
			return new Segment[sourceSegments.Length];
		}

		/// <summary>
		/// Creates a consumable SearchResult using the source segment and translated segment
		/// </summary>
		/// <param name="searchSegment"></param>
		/// <param name="translation"></param>
		/// <param name="sourceSegment"></param>
		/// <returns></returns>
		private static SearchResult CreateSearchResult(Segment searchSegment, Segment translation)
		{
			_logger.Trace("");
			var unit = new TranslationUnit
			{
				SourceSegment = searchSegment,
				TargetSegment = translation,
				ConfirmationLevel = ConfirmationLevel.Translated,
				Origin = TranslationUnitOrigin.Nmt
			};
			unit.ResourceId = new PersistentObjectToken(unit.GetHashCode(), Guid.Empty);
			var searchResult = new SearchResult(unit);

			// We do not currently support scoring, so always say that we're 25% sure on this translation.
			searchResult.ScoringResult = new ScoringResult() { BaseScore = 25 };

			return searchResult;
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
			_logger.Trace("");

			var results = new SearchResults[segments.Length];
			var translations = TranslateSegments(segments.Where((seg, i) => mask == null || mask[i]).ToArray());
			if (!translations.All(translation => translation == null))
			{
				int translationIndex = 0;
				for (int i = 0; i < segments.Length; i++)
				{
					if (mask != null && !mask[i])
					{
						results[i] = null;
						continue;
					}
					results[i] = new SearchResults();
					if (segments[i] != null)
					{
						results[i].SourceSegment = segments[i].Duplicate();
						results[i].Add(CreateSearchResult(segments[i], translations[translationIndex]));
						translationIndex++;
					}
					else
					{
						results[i].SourceSegment = new Segment();
						results[i].Add(CreateSearchResult(new Segment(), new Segment()));
					}
				}
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
			_logger.Trace("");
			if (segments == null)
			{
				throw new ArgumentNullException("segments", "Segments in SearchSegmentsMasked");
			}
			if (mask == null || mask.Length != segments.Length)
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
			_logger.Trace("");
			var seg = new Segment(languageDirection.SourceCulture);
			seg.Add(segment);
			return SearchSegment(settings, seg);
		}

		/// <summary>
		/// Translate a translation unit using the source segment.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="translationUnit"></param>
		/// <returns></returns>
		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			_logger.Trace("");
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
			_logger.Trace("");
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
		public SearchResults[] SearchTranslationUnitsMasked(
			SearchSettings settings,
			TranslationUnit[] translationUnits,
			bool[] mask)
		{
			_logger.Trace("");
			if (translationUnits == null)
			{
				throw new ArgumentNullException("translationUnits", "TranslationUnits in SearchSegmentsMasked");
			}
			if (mask == null || mask.Length != translationUnits.Length)
			{
				throw new ArgumentException("Mask in SearchSegmentsMasked");
			}

			return SearchSegments(settings, translationUnits.Select(tu => tu?.SourceSegment).ToArray(), mask);
		}

		/// <summary>
		/// Prepares a source translation string for request. It ultimately removes all the tags data,
		/// storing them temporarily in a dictionary, which then can be used to reinstate the tags' text.
		/// Function taken from previous MTEdge Plugin.
		/// </summary>
		/// <param name="segments"></param>
		/// <returns>Xliff</returns>
		public Xliff CreateXliffFile(Segment[] segments)
		{
			var file = new File
			{
				SourceCulture = languageDirection.SourceCulture,
				TargetCulture = languageDirection.TargetCulture
			};

			var xliffDocument = new Xliff
			{
				File = file
			};

			foreach (var seg in segments)
			{
				if (seg != null)
				{
					xliffDocument.AddSourceSegment(seg);
				}
			}

			return xliffDocument;
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
		public ImportResult[] AddTranslationUnitsMasked(
			TranslationUnit[] translationUnits,
			ImportSettings settings,
			bool[] mask)
		{
			_logger.Trace("");
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
			_logger.Trace("");
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
			_logger.Trace("");
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
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(
			TranslationUnit[] translationUnits,
			int[] previousTranslationHashes,
			ImportSettings settings,
			bool[] mask)
		{
			_logger.Trace("");
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
			_logger.Trace("");
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
			_logger.Trace("");
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
		public ImportResult[] AddOrUpdateTranslationUnits(
			TranslationUnit[] translationUnits,
			int[] previousTranslationHashes,
			ImportSettings settings)
		{
			_logger.Trace("");
			throw new NotImplementedException();
		}
		#endregion
	}
}
