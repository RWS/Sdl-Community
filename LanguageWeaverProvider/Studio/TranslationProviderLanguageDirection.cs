using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.XliffConverter.Converter;
using LanguageWeaverProvider.XliffConverter.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;

namespace LanguageWeaverProvider
{
	public class TranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly ITranslationOptions _translationOptions;
		private readonly LanguagePair _languagePair;

		private TranslationUnit _currentTranslationUnit;
		private Window _batchTaskWindow;

		public TranslationProviderLanguageDirection(ITranslationProvider translationProvider, ITranslationOptions translationOptions, LanguagePair languagePair, ITranslationProviderCredentialStore credentialStore)
		{
			TranslationProvider = translationProvider;
			_translationOptions = translationOptions;
			_languagePair = languagePair;
			CredentialManager.SetCredentials(credentialStore, translationOptions);
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

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			var searchResults = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; ++i)
			{
				searchResults[i] = SearchSegment(settings, segments[i]);
			}

			return searchResults;
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			if (!_translationOptions.ProviderSettings.ResendDrafts && _currentTranslationUnit.ConfirmationLevel != ConfirmationLevel.Unspecified)
			{
				var targetSegment = new Segment(_languagePair.TargetCulture);
				targetSegment.Add(PluginResources.TranslationDraftNotResent);

				var sR = new SearchResults { SourceSegment = segment.Duplicate() };
				sR.Add(CreateSearchResult(segment, targetSegment));
				return sR;
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
			CredentialManager.ValidateToken(_translationOptions);
			var xliff = CreateXliffFile(new Segment[] { sourceSegment });
			var mappedPair = GetMappedPair();
			var translation = CloudService.Translate(_translationOptions.AccessToken, mappedPair, xliff).Result;
			var translatedSegment = translation.GetTargetSegments().First();
			var searchResult = CreateSearchResult(segment, translatedSegment.Segment);
			if (translatedSegment.Estimation != QualityEstimations.None)
			{
				SetQualityEstimationOnSegment(translatedSegment, mappedPair);
			}

			return searchResult;
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			_batchTaskWindow = Application.Current.Dispatcher.Invoke(ApplicationInitializer.GetBatchTaskWindow);

			var searchResults = new SearchResults[mask.Length];
			var segments = translationUnits.Select(x => x.SourceSegment).ToList();
			var translatableSegments = new List<Segment>();
			for (var i = 0; i < translationUnits.Length; i++)
			{
				_currentTranslationUnit = translationUnits[i];

				var translationUnit = translationUnits[i];
				var currentSegment = segments[i];

				if (!mask[i] || currentSegment is null)
				{
					searchResults[i] = new()
					{
						SourceSegment = new Segment()
					};

					searchResults[i].Add(CreateSearchResult(new Segment(), new Segment()));
					continue;
				}

				if (!_translationOptions.ProviderSettings.ResendDrafts && _currentTranslationUnit.ConfirmationLevel != ConfirmationLevel.Unspecified)
				{
					var targetSegment = new Segment(_languagePair.TargetCulture);
					targetSegment.Add(PluginResources.TranslationDraftNotResent);

					searchResults[i] = new()
					{
						SourceSegment = currentSegment.Duplicate()
					};

					searchResults[i].Add(CreateSearchResult(currentSegment, targetSegment));
					continue;
				}

				translatableSegments.Add(translationUnit.SourceSegment);
			}

			if (!translatableSegments.Any())
			{
				return searchResults;
			}

			CredentialManager.ValidateToken(_translationOptions);
			var mappedPair = GetMappedPair();
			var xliffFile = CreateXliffFile(translatableSegments);
			var translation = CloudService.Translate(_translationOptions.AccessToken, mappedPair, xliffFile).Result;
			var translatedSegments = translation.GetTargetSegments();
			var translatedSegmentsIndex = 0;
			for (var i = 0; i < mask.Length; i++)
			{
				if (searchResults[i] is not null)
				{
					continue;
				}

				_currentTranslationUnit = translationUnits[i];
				var currentSegment = segments[i];

				if (!mask[i]
				 || currentSegment is null
				 || (!_translationOptions.ProviderSettings.ResendDrafts && _currentTranslationUnit.ConfirmationLevel != ConfirmationLevel.Unspecified))
				{
					continue;
				}

				var translatedSegment = translatedSegments[translatedSegmentsIndex++];
				searchResults[i] = new()
				{
					SourceSegment = currentSegment.Duplicate()
				};
				
				searchResults[i].Add(CreateSearchResult(currentSegment, translatedSegment.Segment));
				if (translatedSegment.Estimation != QualityEstimations.None)
				{
					SetQualityEstimationOnSegment(translatedSegment, mappedPair);
				}
			}

			_batchTaskWindow = null;
			return searchResults;
		}

		private void SetQualityEstimationOnSegment(EvaluatedSegment evaluatedSegment, PairMapping pairMapping)
		{
			if (_batchTaskWindow is not null)
			{
				StoreMetadata(evaluatedSegment, pairMapping);
				return;
			}

			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			if (editorController.ActiveDocument is null)
			{
				return;
			}

			var activeSegmentPair = editorController.ActiveDocument.ActiveSegmentPair;
			if (activeSegmentPair is null)
			{
				return;
			}

			activeSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_QE, evaluatedSegment.QualityEstimation);
			activeSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_ShortModelName, pairMapping.SelectedModel.Model);
			activeSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_LongModelName, pairMapping.SelectedModel.Name);
			activeSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_Translation, evaluatedSegment.Segment.ToString());
			editorController.ActiveDocument.UpdateSegmentPairProperties(activeSegmentPair, activeSegmentPair.Properties);
		}

		private void StoreMetadata(EvaluatedSegment evaluatedSegment, PairMapping pairMapping)
		{
			RatedSegments.Segments ??= new List<RatedSegment>();
			var ratedSegment = new RatedSegment()
			{
				Model = pairMapping.SelectedModel.Model,
				ModelName = pairMapping.SelectedModel.Name,
				Translation = evaluatedSegment.Segment.ToString(),
				QualityEstimation = evaluatedSegment.QualityEstimation,
				SegmentId = _currentTranslationUnit.DocumentSegmentPair.Properties.Id,
				TargetLanguageCode = pairMapping.TargetCode
			};

			RatedSegments.Segments.Add(ratedSegment);
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

		public Xliff CreateXliffFile(IEnumerable<Segment> segments)
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

			foreach (var segment in segments)
			{
				if (segment is not null)
				{
					xliffDocument.AddSourceSegment(segment);
				}
			}

			return xliffDocument;
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