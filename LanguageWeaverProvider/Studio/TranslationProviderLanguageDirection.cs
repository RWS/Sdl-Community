using System;
using System.Linq;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.XliffConverter.Converter;
using LanguageWeaverProvider.XliffConverter.Model;
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
			if (!_translationOptions.ProviderSettings.ResendDrafts && _currentTranslationUnit.ConfirmationLevel != ConfirmationLevel.Unspecified)
			{
				var targetSegment = new Segment(TargetLanguage);
				targetSegment.Add(PluginResources.TranslationDraftNotResent);
				return new SearchResults() { CreateSearchResult(segment, targetSegment) } ;
			}

			var sourceSegment = _translationOptions.ProviderSettings.IncludeTags
							  ? segment.Duplicate()
							  : RemoveTagsOnSegment(segment);
			var searchResults = new SearchResults { SourceSegment = sourceSegment };
			var searchResult = TranslateSegment(segment, sourceSegment);
			searchResults.Add(searchResult);
			return searchResults;
		}

		private SearchResult TranslateSegment(Segment segment, Segment sourceSegment)
		{
			var xliff = CreateXliffFile(sourceSegment);
			var mappedPair = GetMappedPair();
			var translation = CloudService.Translate(_translationOptions.CloudCredentials, mappedPair, xliff).Result;
			var translatedSegment = translation.GetTargetSegments();
			var searchResult = CreateSearchResult(segment, translatedSegment[0].Segment);
			return searchResult;
		}

		private Segment RemoveTagsOnSegment(Segment segment)
		{
			var taglessSegment = segment.Duplicate();
			var elements = segment.Duplicate().Elements;
			foreach (var element in elements)
			{
				if (element.GetType() == typeof(Tag))
				{
					taglessSegment.Elements.Remove(element);
				}
			}

			return taglessSegment;
		}

		private PairMapping GetMappedPair()
		{
			return _translationOptions
				   .PairMappings
				   .FirstOrDefault(x => x.LanguagePair.SourceCultureName.Equals(SourceLanguage.Name)
									 && x.LanguagePair.TargetCultureName.Equals(TargetLanguage.Name));
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

			if (segment is not null)
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