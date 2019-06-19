using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.Toolkit.LanguagePlatform.XliffConverter;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly BeGlobalTranslationProvider _beGlobalTranslationProvider;
		private readonly BeGlobalTranslationOptions _options;
		private readonly LanguagePair _languageDirection;
		private readonly List<TranslationUnit> _translationUnits;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		public ITranslationProvider TranslationProvider => _beGlobalTranslationProvider;
		public CultureInfo SourceLanguage { get; }
		public CultureInfo TargetLanguage { get; }
		public bool CanReverseLanguageDirection { get; }

		public BeGlobalLanguageDirection(BeGlobalTranslationProvider beGlobalTranslationProvider, LanguagePair languageDirection)
		{
			_beGlobalTranslationProvider = beGlobalTranslationProvider;
			_languageDirection = languageDirection;
			_options = beGlobalTranslationProvider.Options;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			_translationUnits = new List<TranslationUnit>();
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			return null;
		}

		private Segment[] TranslateSegments(Segment[] sourceSegments)
		{
			var xliffDocument = CreateXliffFile(sourceSegments);

			var sourceLanguage =
				_normalizeSourceTextHelper.GetCorespondingLangCode(_languageDirection.SourceCulture);
			var targetLanguage =
				_normalizeSourceTextHelper.GetCorespondingLangCode(_languageDirection.TargetCulture);

			var translatedXliffText = WebUtility.UrlDecode(_options.BeGlobalService.TranslateText(xliffDocument.ToString(), sourceLanguage, targetLanguage));

			var translatedXliff = Converter.ParseXliffString(translatedXliffText);
			if (translatedXliff != null)
			{
				return translatedXliff.GetTargetSegments();
			}
			return new Segment[sourceSegments.Length];
		}

		public Xliff CreateXliffFile(Segment[] segments)
		{
			var xliffDocument = new Xliff(SourceLanguage, TargetLanguage);

			foreach (var seg in segments)
			{
				if (seg != null)
				{
					xliffDocument.AddSourceSegment(seg);
				}
			}
			return xliffDocument;
		}

		private SearchResult CreateSearchResult(Segment segment, Segment translation)
		{
			var tu = new TranslationUnit
			{
				SourceSegment = segment.Duplicate(),//this makes the original source segment, with tags, appear in the search window
				TargetSegment = translation
			};

			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

			const int score = 0; //score to 0...change if needed to support scoring
			tu.Origin = TranslationUnitOrigin.MachineTranslation;
			var searchResult = new SearchResult(tu)
			{
				ScoringResult = new ScoringResult
				{
					BaseScore = score
				}
			};
			tu.ConfirmationLevel = ConfirmationLevel.Draft;

			return searchResult;
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
			var segmentsToTranslate = segments.Where((seg, i) => mask == null || mask[i]).ToArray();
			var translationIndex = 0;
			var results = new SearchResults[segments.Length];
			if (!_options.ResendDrafts)
			{
				var intermediarSegments = new List<Segment>();
				foreach (var segment in segmentsToTranslate)
				{
					var corespondingTu = _translationUnits.FirstOrDefault(tu => tu.SourceSegment.Equals(segment));
					if (corespondingTu != null)
					{
						if (corespondingTu.ConfirmationLevel != ConfirmationLevel.Unspecified)
						{

							var intermediarTranslations = TranslateSegments(intermediarSegments.ToArray());
							AddTranslationToResults(intermediarTranslations, intermediarSegments.ToArray(), results, translationIndex, mask);

							//add current TU as search result
							var translation = new Segment(_languageDirection.TargetCulture);
							translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);

							var currentTu = new List<Segment>
							{
								translation
							};
							AddTranslationToResults(currentTu.ToArray(), segments, results, translationIndex, mask);

							Array.Clear(intermediarTranslations, 0, intermediarTranslations.Length);
							intermediarSegments.Clear();

						}
						else
						{
							intermediarSegments.Add(segment);
						}
					}
				}
				if (intermediarSegments.Count > 0)
				{
					var intermediarTranslations = TranslateSegments(intermediarSegments.ToArray());
					AddTranslationToResults(intermediarTranslations, segments, results, translationIndex, mask);
				}
			}
			else
			{
				var translations = TranslateSegments(segmentsToTranslate);
				AddTranslationToResults(translations, segments, results, translationIndex, mask);
			}

			return results;
		}
		private void AddTranslationToResults(Segment[] translations, Segment[] segments, SearchResults[] results, int translationIndex, bool[] mask)
		{
			if (translations.Any(translation => translation != null))
			{
				for (var i = 0; i < segments.Length; i++)
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
		}
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			// Need this vs having mask parameter default to null as inheritence doesn't allow default values to
			// count as the same thing as having no parameter at all. IE, you can't have
			// public string foo(string s = null) override public string foo().

			return SearchSegments(settings, segments, null);
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			var results = new SearchResults[segments.Length];
			var translations = TranslateSegments(segments.Where((seg, i) => mask == null || mask[i]).ToArray());
			if (translations.Any(translation => translation != null))
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

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			_translationUnits.Clear();
			_translationUnits.AddRange(translationUnits);
			return null;
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits,
			bool[] mask)
		{
			if (translationUnits == null)
				throw new ArgumentNullException(nameof(translationUnits), @"TranslationUnits in SearchSegmentsMasked");
			if (mask == null || mask.Length != translationUnits.Length)
				throw new ArgumentException("Mask in SearchSegmentsMasked");

			_translationUnits.Clear();
			_translationUnits.AddRange(translationUnits);
			return SearchSegments(settings, translationUnits.Select(tu => tu?.SourceSegment).ToArray(), mask);
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits,
			ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}
	}
}