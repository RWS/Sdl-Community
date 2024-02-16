using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
		private readonly LWSegmentEditor _postLookupEditor;
		private readonly LWSegmentEditor _preLookupEditor;
		private readonly LanguagePair _languagePair;

		private ITranslationOptions _translationOptions;
		private TranslationUnit _currentTranslationUnit;
		private Window _batchTaskWindow;

		public TranslationProviderLanguageDirection(ITranslationProvider translationProvider, ITranslationOptions translationOptions, LanguagePair languagePair)
		{
			TranslationProvider = translationProvider;
			_translationOptions = translationOptions;
			_languagePair = languagePair;
			CredentialManager.GetCredentials(translationOptions, true);

			if (_translationOptions.ProviderSettings.UsePrelookup)
			{
				_preLookupEditor = new(_translationOptions.ProviderSettings.PreLookupFilePath);
			}

			if (_translationOptions.ProviderSettings.UsePostLookup)
			{
				_postLookupEditor = new(_translationOptions.ProviderSettings.PostLookupFilePath);
			}
		}

		public ITranslationProvider TranslationProvider { get; private set; }

		public CultureCode SourceLanguage => _languagePair.SourceCulture;

		public CultureCode TargetLanguage => _languagePair.TargetCulture;

		public bool CanReverseLanguageDirection => false;

		public bool UsePreLookup => _preLookupEditor is not null;

		public bool UsePostLookup => _postLookupEditor is not null;

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			_currentTranslationUnit = translationUnit;
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			return segments.Select(segment => SearchSegment(settings, segment)).ToArray();
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			if (ShouldResendDrafts())
			{
				return CreateDraftNotResentSearchResults(segment);
			}

			var sourceSegment = GetSourceSegment(segment);
			var searchResults = new SearchResults { SourceSegment = sourceSegment };
			searchResults.Add(TranslateSegment(segment, sourceSegment));
			return searchResults;
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			ApplicationInitializer.TranslationOptions ??= new Dictionary<string, ITranslationOptions>();
			if (ApplicationInitializer.TranslationOptions.TryGetValue(_translationOptions.Id, out var currentOptions))
			{
				_translationOptions = currentOptions;
			}
			ManageBatchTaskWindow(true);
			var searchResults = new SearchResults[mask.Length];
			var segmentsInput = translationUnits.Select(x => x.SourceSegment).ToList();
			var translatableSegments = ExtractTranslatableSegments(translationUnits, mask, searchResults, segmentsInput);
   
			if (!translatableSegments.Any())
			{
				ManageBatchTaskWindow();
				return searchResults;
			}

			var (Segments, Emojis) = FilterSegmentEmojis(translatableSegments);

			if (UsePreLookup)
			{
				Segments = ModifySegmentsOnLookup(_preLookupEditor, Segments);
			}

			var mappedPair = GetMappedPair();
			var xliffFile = CreateXliffFile(Segments);
			var translation = GetTranslation(mappedPair, xliffFile);
			var evaluatedSegments = translation.GetTargetSegments();
			var translatedSegments = evaluatedSegments.Select(seg => seg.Segment).ToList();
			if (Emojis.Any())
			{
				ReconstructBaseSegments(translatedSegments, Emojis);
			}

			if (UsePostLookup)
			{
				translatedSegments = ModifySegmentsOnLookup(_postLookupEditor, translatedSegments);
			}

			var fileName = _batchTaskWindow is null ? string.Empty : System.IO.Path.GetFileName(translationUnits.First().DocumentProperties.LastOpenedAsPath);
			var translatedSegmentsIndex = 0;
			for (var i = 0; i < mask.Length; i++)
			{
				if (ShouldSkipSearchResult(searchResults[i], mask[i], segmentsInput[i]))
				{
					continue;
				}

				_currentTranslationUnit = translationUnits[i];
				var currentSegment = Segments[translatedSegmentsIndex];
				var evaluatedSegment = evaluatedSegments[translatedSegmentsIndex];
				var translatedSegment = translatedSegments[translatedSegmentsIndex++];

				searchResults[i] = new SearchResults { SourceSegment = currentSegment.Duplicate() };
				searchResults[i].Add(CreateSearchResult(currentSegment, translatedSegment));
				SetMetadataOnSegment(evaluatedSegment, mappedPair, fileName);
			}

			ManageBatchTaskWindow();
			return searchResults;
		}

		private List<Segment> ModifySegmentsOnLookup(LWSegmentEditor segmentEditor, List<Segment> segments)
		{
			var editedSegments = new List<Segment>();
			foreach (var inSegment in segments)
			{
				var segment = new Segment(inSegment.Culture);
				foreach (var element in inSegment.Elements)
				{
					if (element.GetType() == typeof(Tag))
					{
						segment.Add(element);
						continue;
					}

					segment.Add(segmentEditor.EditText(element.ToString()));
				}

				editedSegments.Add(segment);
			}

			return editedSegments;
		}

		private bool ShouldResendDrafts()
		{
			return !_translationOptions.ProviderSettings.ResendDrafts &&
				   _currentTranslationUnit?.ConfirmationLevel != ConfirmationLevel.Unspecified;
		}

		private SearchResults CreateDraftNotResentSearchResults(Segment segment)
		{
			var targetSegment = new Segment(_languagePair.TargetCulture);
			targetSegment.Add(PluginResources.TranslationDraftNotResent);

			var searchResults = new SearchResults { SourceSegment = segment.Duplicate() };
			searchResults.Add(CreateSearchResult(segment, targetSegment));
			return searchResults;
		}

		private Segment GetSourceSegment(Segment segment)
		{
			return _translationOptions.ProviderSettings.IncludeTags
				 ? segment.Duplicate()
				 : RemoveTagsOnSegment(segment);
		}

		private void ManageBatchTaskWindow(bool initialize = false)
		{
			_batchTaskWindow = initialize ? Application.Current.Dispatcher.Invoke(ApplicationInitializer.GetBatchTaskWindow) : null;
			if (_batchTaskWindow is null)
			{
				return;
			}
		}

		private bool ShouldSkipSearchResult(SearchResults searchResult, bool isMasked, Segment segment)
		{
			return searchResult is not null || !isMasked || segment is null || ShouldResendDrafts();
		}

		private Xliff GetTranslation(PairMapping mappedPair, Xliff xliffFile)
		{
			return _translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud
				 ? CloudService.Translate(_translationOptions.AccessToken, mappedPair, xliffFile).Result
				 : EdgeService.Translate(_translationOptions.AccessToken, mappedPair, xliffFile).Result;
		}

		private SearchResult TranslateSegment(Segment segment, Segment sourceSegment)
		{
			var xliff = CreateXliffFile(new Segment[] { sourceSegment });
			var mappedPair = GetMappedPair();
			var translation = CloudService.Translate(_translationOptions.AccessToken, mappedPair, xliff).Result;
			var translatedSegment = translation.GetTargetSegments().First();
			var searchResult = CreateSearchResult(segment, translatedSegment.Segment);
			SetMetadataOnSegment(translatedSegment, mappedPair, null);

			return searchResult;
		}

		private Xliff CreateXliffFile(IEnumerable<Segment> segments)
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

		private void SetMetadataOnSegment(EvaluatedSegment evaluatedSegment, PairMapping pairMapping, string fileName)
		{
			if (_batchTaskWindow is not null
			 || _currentTranslationUnit.ConfirmationLevel == ConfirmationLevel.Draft)
			{
				StoreSegmentMetadata(evaluatedSegment, pairMapping, fileName);
				return;
			}

			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			if (editorController.ActiveDocument is null)
			{
				return;
			}

			var currentSegmentId = _currentTranslationUnit.DocumentSegmentPair.Properties.Id;
			var currentSegmentPair = editorController.ActiveDocument.SegmentPairs.First(p => p.Properties.Id == currentSegmentId);

			currentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_QE, evaluatedSegment.QualityEstimation);
			currentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_LongModelName, pairMapping.SelectedModel.Name);
			currentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_ShortModelName, pairMapping.SelectedModel.Model);
			currentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_Translation, evaluatedSegment.Segment.ToString());
			currentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_Feedback, _translationOptions.ProviderSettings.AutosendFeedback.ToString());
			editorController.ActiveDocument.UpdateSegmentPairProperties(currentSegmentPair, currentSegmentPair.Properties);
		}

		private void StoreSegmentMetadata(EvaluatedSegment evaluatedSegment, PairMapping pairMapping, string fileName)
		{
			var ratedSegment = new RatedSegment()
			{
				Model = pairMapping.SelectedModel.Model,
				ModelName = pairMapping.SelectedModel.Name,
				Translation = evaluatedSegment.Segment.ToString(),
				QualityEstimation = evaluatedSegment.QualityEstimation,
				SegmentId = _currentTranslationUnit.DocumentSegmentPair.Properties.Id,
				TargetLanguageCode = pairMapping.TargetCode,
				FileName = fileName,
				AutosendFeedback = _translationOptions.ProviderSettings.AutosendFeedback
			};

			var existingSegment = ApplicationInitializer.RatedSegments.FirstOrDefault(x => x.SegmentId.Id.Equals(ratedSegment.SegmentId.Id) && x.FileName.Equals(fileName) && x.ModelName.Equals(ratedSegment.ModelName));
			if (existingSegment is null)
			{
				ApplicationInitializer.RatedSegments.Add(ratedSegment);
			}
			else
			{
				var existingSegmentIndex = ApplicationInitializer.RatedSegments.IndexOf(existingSegment);
				ApplicationInitializer.RatedSegments[existingSegmentIndex] = ratedSegment;
			}
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

		#region To finish
		private List<Segment> ExtractTranslatableSegments(TranslationUnit[] translationUnits, bool[] mask, SearchResults[] searchResults, List<Segment> segments)
		{
			var translatableSegments = new List<Segment>();
			for (var i = 0; i < translationUnits.Length; i++)
			{
				_currentTranslationUnit = translationUnits[i];
				var translationUnit = translationUnits[i];
				var currentSegment = segments[i];

				if (!mask[i] || currentSegment is null)
				{
					searchResults[i] = new SearchResults { SourceSegment = new Segment() };
					continue;
				}

				if (ShouldResendDrafts())
				{
					searchResults[i] = CreateDraftNotResentSearchResults(currentSegment);
					continue;
				}

				var sourceSegment = GetSourceSegment(translationUnit.SourceSegment);
				translatableSegments.Add(sourceSegment);
			}

			return translatableSegments;
		}

		private void ReconstructBaseSegments(List<Segment> translations, List<string> emojis)
		{
			var anchorCount = 1;
			var currentEmojiIndex = 0;
			foreach (var translation in translations)
			{
				for (var i = 0; i < translation.Elements.Count; i++)
				{
					var element = translation.Elements[i];
					if (element is not Tag tag || !tag.TagID.Contains("Emoji"))
					{
						continue;
					}

					if (tag.TagID.Contains("textEmoji"))
					{
						translation.Elements[i] = new Text(emojis[currentEmojiIndex++]);
						continue;
					}

					tag.Anchor = anchorCount++;
					tag.TagID = tag.TextEquivalent;
					tag.TextEquivalent = emojis[currentEmojiIndex++];
				}
			}
		}

		private (List<Segment> Segments, List<string> Emojis) FilterSegmentEmojis(List<Segment> translatableSegments)
		{
			const string EmojiRegexPattern = @"(\p{So}|\p{Cs}\p{Cs}(\p{Cf}\p{Cs}\p{Cs})*)";

			var textTagIndex = 1;
			var newSegments = new List<Segment>();
			var emojis = new List<string>();
			foreach (var segment in translatableSegments)
			{
				var newSegment = new Segment();
				foreach (var element in segment.Elements)
				{
					var input = element.ToString();

					if (element is Tag tag)
					{
						var isEmoji = DetectTagEmoji(tag, emojis);
						if (isEmoji)
						{
							var newTag = CreateEmojiPlaceholder(tag);
							newSegment.Elements.Add(newTag);
							continue;
						}

						newSegment.Elements.Add(element);
						continue;
					}

					if (element is Text text)
					{
						var regex = new Regex(EmojiRegexPattern);
						var splitResults = regex.Split(input);

						foreach (var result in splitResults)
						{
							var isEmoji = regex.Match(result).Success;
							if (isEmoji)
							{
								var newTag = CreateEmojiPlaceholder(textTagIndex++);
								newSegment.Elements.Add(newTag);
								emojis.Add(result);
								continue;
							}

							newSegment.Elements.Add(new Text(result));
						}
					}
				}

				newSegments.Add(newSegment);
			}

			return (newSegments, emojis);
		}

		bool DetectTagEmoji(Tag tag, List<string> emojis)
		{
			var input = tag.ToString();
			var hasEmoji = false;
			for (var i = 0; i < input.Length; i++)
			{
				var category = CharUnicodeInfo.GetUnicodeCategory(input, i);
				if (category == UnicodeCategory.OtherSymbol || category == UnicodeCategory.OtherNotAssigned)
				{
					hasEmoji = true;
					var emoji = char.ConvertFromUtf32(char.ConvertToUtf32(input, i));
					emojis.Add(emoji);
				}
			}

			return hasEmoji;
		}

		Tag CreateEmojiPlaceholder(Tag currentTag)
		{
			return new Tag(
				TagType.TextPlaceholder,
				$"tagEmoji_{currentTag.TagID}",
				0,
				currentTag.AlignmentAnchor,
				currentTag.TagID);
		}

		Tag CreateEmojiPlaceholder(int index)
		{
			return new Tag(
				TagType.TextPlaceholder,
				$"textEmoji_{index}",
				0,
				0,
				index.ToString());
		}
		#endregion

		#region Unused
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

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			throw new NotImplementedException();
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
		#endregion
	}
}