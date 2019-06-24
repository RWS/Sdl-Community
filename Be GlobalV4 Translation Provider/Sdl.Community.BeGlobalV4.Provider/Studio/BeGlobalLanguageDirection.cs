using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
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
		private readonly StudioCredentials _studioCredentials = new StudioCredentials();

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
			//maybe the user logged out since the provider was added or the token expired

			//Application.Current?.Dispatcher?.Invoke(() =>
			//{
			//	_studioCredentials.GetToken();
			//});
			_studioCredentials.GetToken();


			var xliffDocument = CreateXliffFile(sourceSegments);

			var sourceLanguage =
				_normalizeSourceTextHelper.GetCorespondingLangCode(_languageDirection.SourceCulture);
			var targetLanguage =
				_normalizeSourceTextHelper.GetCorespondingLangCode(_languageDirection.TargetCulture);

			var translatedXliffText =
				WebUtility.UrlDecode(
					_options.BeGlobalService.TranslateText(xliffDocument.ToString(), sourceLanguage, targetLanguage));

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
			var results = new SearchResults[segments.Length];

			var beGlobalSegments = new List<BeGlobalSegment>();
			var alreadyTranslatedSegments = new List<BeGlobalSegment>();
			if (!_options.ResendDrafts)
			{
				// Re-send draft segment logic
				for (var i = 0; i < segments.Length; i++)
				{
					if (mask != null && !mask[i])
					{
						results[i] = null;
						continue;
					}
					var corespondingTu = _translationUnits.FirstOrDefault(tu => tu.SourceSegment.Equals(segments[i]));
					//locked segments should not be translated
					if (corespondingTu != null && (corespondingTu.ConfirmationLevel != ConfirmationLevel.Unspecified || corespondingTu.DocumentSegmentPair.Properties.IsLocked))
					{
						var translation = new Segment(_languageDirection.TargetCulture);
						translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);

						var alreadyTranslatedSegment = new BeGlobalSegment
						{
							Translation = translation,
							Segment = segments[i],
							Index = i,
							SearchResult = CreateSearchResult(segments[i], translation)
						};
						alreadyTranslatedSegments.Add(alreadyTranslatedSegment);
					}
					else
					{
						var segmentToBeTranslated = new BeGlobalSegment
						{
							Segment = segments[i],
							Index = i
						};
						beGlobalSegments.Add(segmentToBeTranslated);
					}
				}
				if (beGlobalSegments.Count > 0)
				{
					GetTranslations(beGlobalSegments);
					SetSearchResults(results, beGlobalSegments);
				}
				if (alreadyTranslatedSegments.Count > 0)
				{
					SetSearchResults(results, alreadyTranslatedSegments);
				}
			}
			else
			{
				var translations = TranslateSegments(segments.Where((seg, i) => mask == null || mask[i]).ToArray());

				if (translations.Any(translation => translation != null))
				{
					var translationIndex = 0;
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
			return results;
		}

		private void SetSearchResults(SearchResults[]results,List<BeGlobalSegment> translatedSegments)
		{
			foreach (var segment in translatedSegments)
			{
				if (segment?.Segment != null)
				{
					results[segment.Index] = new SearchResults
					{
						SourceSegment = segment.Segment.Duplicate()
					};
					results[segment.Index].Add(segment.SearchResult);
				}
			}
		}

		private void GetTranslations(List<BeGlobalSegment>beGlobalSegments)
		{
			var segmentsToBeTranslated = new List<Segment>();
			foreach (var segment in beGlobalSegments)
			{
				segmentsToBeTranslated.Add(segment.Segment);
			}
			var translations = TranslateSegments(segmentsToBeTranslated.ToArray());
			for (var i = 0; i < beGlobalSegments.Count; i++)
			{
				beGlobalSegments[i].Translation = translations[i];
				beGlobalSegments[i].SearchResult = CreateSearchResult(beGlobalSegments[i].Segment, translations[i]);
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
