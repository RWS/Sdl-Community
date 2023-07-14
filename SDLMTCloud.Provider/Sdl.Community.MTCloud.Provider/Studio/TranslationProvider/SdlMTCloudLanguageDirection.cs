using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.RateIt;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;
using Xliff = Sdl.Community.MTCloud.Provider.XliffConverter.Converter.Xliff;

namespace Sdl.Community.MTCloud.Provider.Studio.TranslationProvider
{
	public class SdlMTCloudLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly EditorController _editorController;
		private readonly LanguagePair _languageDirection;
		private readonly SdlMTCloudTranslationProvider _translationProvider;
		private readonly List<TranslationUnit> _translationUnits;

		public SdlMTCloudLanguageDirection(SdlMTCloudTranslationProvider translationProvider, LanguagePair languageDirection, EditorController editorController)
		{
			_translationProvider = translationProvider;
			_languageDirection = languageDirection;
			_translationUnits = new List<TranslationUnit>();
			_editorController = editorController;
		}

		public bool CanReverseLanguageDirection { get; }
		public CultureCode SourceLanguage => _languageDirection.SourceCulture;
		public CultureCode TargetLanguage => _languageDirection.TargetCulture;

		//CultureInfo ITranslationProviderLanguageDirection.SourceLanguage => _languageDirection.SourceCulture;

		//CultureInfo ITranslationProviderLanguageDirection.TargetLanguage => _languageDirection.TargetCulture;

		public ITranslationProvider TranslationProvider => _translationProvider;

		private ISegmentPair CurrentSegmentPair => _editorController?.ActiveDocument?.ActiveSegmentPair;

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

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits,
			ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public Xliff CreateXliffFile(Segment[] segments)
		{
			var file = new XliffConverter.Models.File
			{
				SourceCulture = _languageDirection.SourceCulture,
				TargetCulture = _languageDirection.TargetCulture
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

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			return null;
		}

		/// <summary>
		/// Translate an array of segments.
		/// </summary>
		/// <param name="segments">Array of segments to be translated (depending on the value of the
		///     corresponding mask)</param>
		/// <param name="mask">Whether to translate a segment or not</param>
		/// <param name="fileAndSegmentIds"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public SearchResults[] SearchSegments(Segment[] segments, bool[] mask, FileAndSegmentIds fileAndSegmentIds = null)
		{
			var results = new SearchResults[segments.Length];
			var mtCloudSegments = new List<MTCloudSegment>();
			var alreadyTranslatedSegments = new List<MTCloudSegment>();

			if (!_translationProvider.Options.ResendDraft)
			{
				// Re-send draft segment logic
				for (var segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++)
				{
					if (mask != null && !mask[segmentIndex])
					{
						results[segmentIndex] = null;
						continue;
					}
					var existsMergedSegments = CheckMergedSegments(results, CurrentSegmentPair, segmentIndex);
					if (existsMergedSegments)
					{
						continue;
					}

					// Set translation unit based on segment index: when the first 10 segments are translated for the first time,
					// then the TU index is the same as segment index
					var correspondingTu = _translationUnits[segmentIndex];
					var currentSegment = correspondingTu.TargetSegment == null
						? CurrentSegmentPair
						: correspondingTu.DocumentSegmentPair;

					if (IsDraftOrTranslated(currentSegment) && !IsTargetEqualToSource(currentSegment))
					{
						alreadyTranslatedSegments.Add(CreateTranslatedSegment(segments, segmentIndex));
					}
					else
					{
						mtCloudSegments.Add(CreateMTCloudSegments(segments, segmentIndex));
					}
				}

				if (mtCloudSegments.Count > 0)
				{
					RemoveSegmentsThatMustNotBeTranslated(fileAndSegmentIds, mtCloudSegments);

					var hasTranslations = GetTranslations(mtCloudSegments, fileAndSegmentIds);
					if (hasTranslations)
					{
						SetSearchResults(results, mtCloudSegments);
					}
				}

				if (alreadyTranslatedSegments.Count > 0)
				{
					SetSearchResults(results, alreadyTranslatedSegments);
				}
			}
			else
			{
				var translations = TranslateSegments(segments.ToArray(), fileAndSegmentIds);

				if (translations.Any(translation => translation != null))
				{
					var translationIndex = 0;
					for (var i = 0; i < segments.Length; i++)
					{
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

		private static void RemoveSegmentsThatMustNotBeTranslated(FileAndSegmentIds fileAndSegmentIds, List<MTCloudSegment> mtCloudSegments)
		{
			var segmentEntries = new List<(SegmentId, string)>();
			foreach (var index in mtCloudSegments.Select(seg => seg.Index))
			{
				var segmentId = fileAndSegmentIds.Segments.Keys.ElementAt((int)index);
				segmentEntries.Add((segmentId, fileAndSegmentIds.Segments[segmentId]));
			}

			fileAndSegmentIds.Segments = segmentEntries.ToDictionary(se => se.Item1, se => se.Item2);
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			// Need this vs having mask parameter default to null as inheritence doesn't allow default values to
			// count as the same thing as having no parameter at all. IE, you can't have
			// public string foo(string s = null) override public string foo().
			return SearchSegments(segments, null);
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			var results = new SearchResults[segments.Length];
			var translations = TranslateSegments(segments.Where((seg, i) => mask == null || mask[i]).ToArray(), null);
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

		private string GetFilePath(TranslationUnit[] translationUnits)
			=> translationUnits[0].DocumentProperties is not null
				? translationUnits[0].DocumentProperties.LastOpenedAsPath
				: translationUnits.Length > 1
					? translationUnits[1].DocumentProperties.LastOpenedAsPath
					: null;

		public Dictionary<string, List<ISegment>> Segments { get; set; } = new();

		private List<ISegment> GetAllSegments(string filePath)
		{
			if (Segments.ContainsKey(filePath))
				return Segments[filePath];
			var segmentRetriever = new SegmentRetriever();
			var converter = MtCloudApplicationInitializer.FileTypeManager.GetConverterToDefaultBilingual(filePath, filePath, null);
			converter?.AddBilingualProcessor(new BilingualContentHandlerAdapter(segmentRetriever));
			converter?.Parse();

			Segments[filePath] = segmentRetriever.Segments;
			return Segments[filePath];
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits,
			bool[] mask)
		{
			var filePath = GetFilePath(translationUnits);
			var resolvedNullIdSegments = translationUnits.Any(tu => tu.DocumentSegmentPair?.Properties.Id == null)
				? GetResolvedNullIdSegments(translationUnits, filePath)
				: translationUnits.ToDictionary(tu => tu.DocumentSegmentPair.Properties.Id,
					tu => tu.SourceSegment.ToString());

			var sdlMtFileAndSegmentIds = new FileAndSegmentIds
			{
				FilePath = filePath,
				Segments = resolvedNullIdSegments
			};

			if (translationUnits == null)
			{
				throw new ArgumentNullException(nameof(translationUnits), @"TranslationUnits in SearchSegmentsMasked");
			}

			if (mask == null || mask.Length != translationUnits.Length)
			{
				throw new ArgumentException("Mask in SearchSegmentsMasked");
			}

			_translationUnits.Clear();
			_translationUnits.AddRange(translationUnits);

			return SearchSegments(translationUnits.Select(tu => tu?.SourceSegment).ToArray(), mask, sdlMtFileAndSegmentIds);
		}

		private Dictionary<SegmentId, string> GetResolvedNullIdSegments(TranslationUnit[] translationUnits, string filePath)
		{
			var allFileSegments = GetAllSegments(filePath);
			var translationUnitData = translationUnits
					.Select(td => (td.DocumentSegmentPair?.Properties.Id ?? new SegmentId("null"),
						td.SourceSegment.ToString())).ToList<(SegmentId SegmentId, string Text)>();

			//GetAdjacentNullIdSegmentsList
			var nullAdjSeqLists = new List<(int Start, int End)>();
			var previousWasNull = false;
			for (var index = 0; index < translationUnitData.Count; index++)
			{
				var currentTdSegment = translationUnitData[index];

				var currentSegmentId = currentTdSegment.SegmentId.ToString();

				var startOfNullList = -1;
				if (!previousWasNull && currentSegmentId == "null" && index != 0)
				{
					startOfNullList = index;
					previousWasNull = true;
				}

				if (previousWasNull && currentSegmentId != "null")
				{
					previousWasNull = false;
					nullAdjSeqLists.Add((startOfNullList, index - 1));
				}
			}

			foreach (var adjNullSequence in nullAdjSeqLists)
			{
				//EstablishSearchRange
				var tudRange = GetSegmentIdRange(adjNullSequence, translationUnitData, allFileSegments);

				//Resolve
				var correspondingSequence = GetCorrespondingSegmentSequences(tudRange, adjNullSequence,
					translationUnitData, allFileSegments.Select(seg =>
						(seg.Properties.Id, seg.ToString())).ToList());


				foreach (var cSeq in correspondingSequence)
				{
					translationUnitData[cSeq.Item2] = (cSeq.Item1, translationUnitData[cSeq.Item2].Text);
				}
			}

			return translationUnitData.ToDictionary(tud => tud.SegmentId, tud => tud.Text);
		}

		private List<(SegmentId, int)> GetCorrespondingSegmentSequences((SegmentId FirstLimit, SegmentId SecondLimit) tudRange,
			(int Start, int End) adjNullSequence, List<(SegmentId Id, string Text)> translationUnitData,
			List<(SegmentId Id, string Text)> allFileSegments)
		{
			var nullIdSegments = GetNullIdSegments(adjNullSequence, translationUnitData);
			var nullIdSegmentTexts = nullIdSegments.Select(seg => seg.Item1).ToList();

			var correspondingSequence = new List<SegmentId>();
			//NULLs are on the starting positions of the segments sent for translation
			if (tudRange.FirstLimit == allFileSegments[0].Id && tudRange.SecondLimit != allFileSegments[allFileSegments.Count - 1].Id)
			{
				var endOfCorrespondingList = allFileSegments.Select((SegmentData, Index) => (SegmentData, Index))
					.FirstOrDefault(seg => seg.SegmentData.Id == tudRange.SecondLimit).Index;
				var startOfCorrespondingList = endOfCorrespondingList - nullIdSegmentTexts.Count;

				for (var index = startOfCorrespondingList; index < endOfCorrespondingList; index++)
				{
					correspondingSequence.Add(allFileSegments[index].Id);
				}
			}

			//NULLs are in the middle of the segments sent for translation
			if (tudRange.FirstLimit != allFileSegments[0].Id && tudRange.SecondLimit != allFileSegments[allFileSegments.Count - 1].Id)
			{
				var insideRange = false;
				foreach (var fileSegment in allFileSegments)
				{
					if (!insideRange && fileSegment.Id == tudRange.FirstLimit)
						insideRange = true;

					if (insideRange)
					{
						correspondingSequence.Add(fileSegment.Id);
					}

					if (insideRange && fileSegment.Id != tudRange.SecondLimit)
						insideRange = false;
				}
			}

			//NULLs are on the last positions of the segments sent for translation
			if (tudRange.FirstLimit != allFileSegments[0].Id && tudRange.SecondLimit == allFileSegments[allFileSegments.Count - 1].Id)
			{
				var startOfCorrespondingList = allFileSegments.Select((SegmentData, Index) => (SegmentData, Index))
					.FirstOrDefault(seg => seg.SegmentData.Id == tudRange.FirstLimit).Index;
				var endOfCorrespondingList = startOfCorrespondingList + nullIdSegmentTexts.Count + 1;

				for (var index = startOfCorrespondingList; index < endOfCorrespondingList; index++)
				{
					correspondingSequence.Add(allFileSegments[index].Id);
				}
			}

			var resolvedList = new List<(SegmentId, int)>(correspondingSequence.Count);

			var i = 0;
			var indices = nullIdSegments.Select(seg => seg.Item2).ToList();
			resolvedList.AddRange(correspondingSequence.Select(cSeq => (cSeq, indices[i++])));

			return resolvedList;
		}

		private static List<(string, int)> GetNullIdSegments((int Start, int End) adjNullSequence, List<(SegmentId Id, string Text)> translationUnitData)
		{
			var startIndex = adjNullSequence.Start < 0 ? 0 : adjNullSequence.Start;
			var nullIdSegments = new List<(string, int)>(adjNullSequence.End - adjNullSequence.Start + 1);
			for (var i = startIndex; i <= adjNullSequence.End; i++)
				nullIdSegments.Add((translationUnitData[i].Text, i));
			return nullIdSegments;
		}

		private (SegmentId, SegmentId) GetSegmentIdRange((int Start, int End) adjacentNullSequence, List<(SegmentId Id, string Text)> translationUnitData, List<ISegment> allFileSegments)
		{
			var previousIndex = adjacentNullSequence.Start - 1;
			var previousNonNullSegmentId = previousIndex > -1
				? translationUnitData[previousIndex].Id
				: allFileSegments[0].Properties.Id;

			var nextIndex = adjacentNullSequence.End + 1;
			var nextNonNullSegmentId = nextIndex < translationUnitData.Count
				? translationUnitData[nextIndex].Id
				: allFileSegments[allFileSegments.Count - 1].Properties.Id;

			return (previousNonNullSegmentId, nextNonNullSegmentId);
		}

		public Segment[] TranslateSegments(Segment[] sourceSegments, FileAndSegmentIds fileAndSegmentIds)
		{
			var xliffDocument = CreateXliffFile(sourceSegments);

			var model = _translationProvider.Options.LanguageMappings?.FirstOrDefault(l =>
				l.SourceTradosCode.Equals(_languageDirection?.SourceCulture?.Name, StringComparison.InvariantCultureIgnoreCase) &&
			 		l.TargetTradosCode.Equals(_languageDirection?.TargetCulture?.Name, StringComparison.InvariantCultureIgnoreCase));

			var targetSegments = Task.Run(async () => await _translationProvider.TranslationService.TranslateText(
				xliffDocument.ToString(), model, fileAndSegmentIds)).Result;

			return targetSegments ?? new Segment[0];
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

		// Ignore when translations exists and segments are merged.
		// When segments are merged, always the last one becomes empty and is removed. Eg: when merging segment 4 and 5, the text is added inside segment 4,
		// and segment 5 becomes empty and hidden from Editor
		private bool CheckMergedSegments(SearchResults[] results, ISegmentPair activeSegmentPair, int segmentIndex)
		{
			if (activeSegmentPair != null)
			{
				var activeSegmentPairId = int.TryParse(activeSegmentPair.Target.Properties.Id.Id, out _)
					? int.Parse(activeSegmentPair.Target.Properties.Id.Id)
					: 0;

				var nextSegmentPair = _editorController?.ActiveDocument?.SegmentPairs?.SkipWhile(
					s => s.Properties.Id.Id != activeSegmentPairId.ToString()).Skip(1).FirstOrDefault();

				if (activeSegmentPair.Target.Count > 0 && (nextSegmentPair == null || !nextSegmentPair.Source.Any()))
				{
					results[segmentIndex] = null;
					return true;
				}
			}

			return false;
		}

		// Set the segments used to receive the translations from server
		private MTCloudSegment CreateMTCloudSegments(Segment[] segments, int segmentIndex)
		{
			var segmentToBeTranslated = new MTCloudSegment
			{
				Segment = segments[segmentIndex],
				Index = segmentIndex
			};

			return segmentToBeTranslated;
		}

		private SearchResult CreateSearchResult(Segment segment, Segment translation)
		{
			var tu = new TranslationUnit
			{
				SourceSegment = segment.Duplicate(),//this makes the original source segment, with tags, appear in the search window
				TargetSegment = translation ?? new Segment()
			};

			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

			const int score = 0; //score to 0...change if needed to support scoring

			tu.Origin = TranslationUnitOrigin.Nmt;

			var searchResult = new SearchResult(tu)
			{
				ScoringResult = new ScoringResult
				{
					BaseScore = score
				},
				TranslationProposal = tu
			};

			tu.ConfirmationLevel = ConfirmationLevel.Draft;

			return searchResult;
		}

		// Create the already translated segments in case the translation was already received from the server
		private MTCloudSegment CreateTranslatedSegment(Segment[] segments, int segmentIndex)
		{
			var translation = new Segment(_languageDirection.TargetCulture);
			translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);

			var alreadyTranslatedSegment = new MTCloudSegment
			{
				Translation = translation,
				Segment = segments[segmentIndex],
				Index = segmentIndex,
				SearchResult = CreateSearchResult(segments[segmentIndex], translation)
			};

			return alreadyTranslatedSegment;
		}

		private bool GetTranslations(IReadOnlyList<MTCloudSegment> mtCloudSegments, FileAndSegmentIds fileAndSegmentIds)
		{
			var translations = TranslateSegments(mtCloudSegments.Select(segment => segment.Segment).ToArray(), fileAndSegmentIds);

			if (translations.Any() && translations[0] != null)
			{
				for (var i = 0; i < mtCloudSegments.Count; i++)
				{
					mtCloudSegments[i].Translation = translations[i];
					mtCloudSegments[i].SearchResult = CreateSearchResult(mtCloudSegments[i].Segment, translations[i]);
				}

				return true;
			}

			return false;
		}

		private bool IsDraftOrTranslated(ISegmentPair segmentPair)
			=> segmentPair.Target?.Count > 0 || segmentPair.Properties.IsLocked;

		private bool IsTargetEqualToSource(ISegmentPair segmentPair)
			=> segmentPair.Source.ToString().Equals(segmentPair.Target.ToString());

		private void SetSearchResults(SearchResults[] results, IEnumerable<MTCloudSegment> translatedSegments)
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
	}
}