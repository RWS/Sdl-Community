using System;
using System.Globalization;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.NewFolder;
using LanguageWeaverProvider.XliffConverter.Converter;
using LanguageWeaverProvider.XliffConverter.Models;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;

namespace LanguageWeaverProvider
{
	public class TranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly ITranslationOptions _translationOptions;
		private readonly LanguagePair _languagePair;
		private TranslationUnit _currentTranslationUnit;

		public TranslationProviderLanguageDirection(ITranslationProvider translationProvider, ITranslationOptions translationOptions, LanguagePair languagePair)
		{
			TranslationProvider = translationProvider;
			_translationOptions = translationOptions;
			_languagePair = languagePair;
		}

		public ITranslationProvider TranslationProvider { get; private set; }

		public CultureCode SourceLanguage => _languagePair.SourceCulture;

		public CultureCode TargetLanguage => _languagePair.TargetCulture;

		public bool CanReverseLanguageDirection => false;

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			var searchResults = new SearchResults { SourceSegment = segment.Duplicate() };
			var cloudService = new CloudService();

			var xliff = CreateXliffFile(segment);
			var translation = cloudService.Translate(_translationOptions.CloudCredentials, xliff).Result;
			var translatedSegment = translation.GetTargetSegments();

			searchResults.Add(CreateSearchResult(segment, translatedSegment[0].Segment));
			return searchResults;
		}

		public Xliff CreateXliffFile(Segment segment)
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

			if (segment != null)
			{
				xliffDocument.AddSourceSegment(segment);
			}

			return xliffDocument;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			var searchResults = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; ++i)
			{
				searchResults[i] = SearchSegment(settings, segments[i]);
			}

			return searchResults;
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			_currentTranslationUnit = translationUnit;
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			var searchResults = new SearchResults[mask.Length];
			for (var i = 0; i < translationUnits.Length; i++)
			{
				searchResults[i] = mask[i] ? SearchTranslationUnit(settings, translationUnits[i])
										   : null;
			}

			return searchResults;
		}


		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

		private SearchResult CreateSearchResult(Segment searchSegment, Segment translation)
		{
			var translationUnit = new TranslationUnit
			{
				ConfirmationLevel = ConfirmationLevel.Draft,
				Origin = TranslationUnitOrigin.Nmt,
				SourceSegment = searchSegment.Duplicate(),
				TargetSegment = translation
			};

			translationUnit.ResourceId = new PersistentObjectToken(translationUnit.GetHashCode(), Guid.Empty);
			return new SearchResult(translationUnit)
			{
				ScoringResult = new ScoringResult { BaseScore = 0 },
				TranslationProposal = new TranslationUnit(translationUnit)
			};
		}
	}
}