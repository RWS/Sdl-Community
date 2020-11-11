﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.XliffConverter.Models;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using TranslationUnit = Sdl.LanguagePlatform.TranslationMemory.TranslationUnit;
using Xliff = Sdl.Community.MTCloud.Provider.XliffConverter.Converter.Xliff;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class SdlMTCloudLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly SdlMTCloudTranslationProvider _translationProvider;
		private readonly LanguagePair _languageDirection;
		private readonly List<TranslationUnit> _translationUnits;
		private readonly EditorController _editorController;

		public SdlMTCloudLanguageDirection(SdlMTCloudTranslationProvider translationProvider, LanguagePair languageDirection, EditorController editorController)
		{
			_translationProvider = translationProvider;
			_languageDirection = languageDirection;
			_translationUnits = new List<TranslationUnit>();
			_editorController = editorController;
		}

		public ITranslationProvider TranslationProvider => _translationProvider;

		public CultureInfo SourceLanguage { get; }

		public CultureInfo TargetLanguage { get; }

		public bool CanReverseLanguageDirection { get; }

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			return null;
		}

		public Segment[] TranslateSegments(Segment[] sourceSegments)
		{
			var xliffDocument = CreateXliffFile(sourceSegments);

			var model = _translationProvider.Options.LanguageMappings?.FirstOrDefault(l =>
				l.SourceTradosCode.Equals(_languageDirection?.SourceCulture?.Name, StringComparison.InvariantCultureIgnoreCase) &&
			 		l.TargetTradosCode.Equals(_languageDirection?.TargetCulture?.Name, StringComparison.InvariantCultureIgnoreCase));

			var targetSegments = Task.Run(async () => await _translationProvider.TranslationService.TranslateText(
				xliffDocument.ToString(), model)).Result;

			return targetSegments ?? new Segment[0];
		}

		public Xliff CreateXliffFile(Segment[] segments)
		{
			var file = new File
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

		/// <summary>
		/// Translate an array of segments.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="segments">Array of segments to be translated (depending on the truthfulness of
		/// corresponding mask)</param>
		/// <param name="mask">Whether to translate a segment or not</param>
		/// <returns></returns>
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments, bool[] mask)
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

					var activeSegmentPair = _editorController?.ActiveDocument?.ActiveSegmentPair;
					var existsMergedSegments = CheckMergedSegments(results, activeSegmentPair, segmentIndex);
					if (existsMergedSegments)
					{
						continue;
					}

					// Set translation unit based on segment index: when the first 10 segments are translated for the first time,
					// then the TU index is the same as segment index
					var correspondingTu = _translationUnits[segmentIndex];

					// If activeSegmentPair is not null, it means the user translates segments through Editor
					// If activeSegmentPair is null, it means the user executes Pre-Translate Batch task, so he does not navigate through segments in editor
					var documentLastOpenPath = _translationUnits[0]?.DocumentProperties?.LastOpenedAsPath;
					if (documentLastOpenPath == null || documentLastOpenPath.Equals(_editorController?.ActiveDocument?.ActiveFile?.LocalFilePath))
					{
						if (activeSegmentPair != null && (activeSegmentPair.Target.Count > 0 || activeSegmentPair.Properties.IsLocked))
						{
							alreadyTranslatedSegments.Add(CreateTranslatedSegment(segments, segmentIndex));
						}
						// In case user copies the source to target and run the pre-translation, do nothing and continue the flow.
						else if (correspondingTu != null && IsSameSourceTarget(correspondingTu))
						{
							// do nothing
						}
						// If is already translated or is locked, then the request to server should not be done and it should not be translated
						else if (activeSegmentPair == null && correspondingTu != null && (correspondingTu.DocumentSegmentPair.Target.Count > 0 || correspondingTu.DocumentSegmentPair.Properties.IsLocked))
						{
							alreadyTranslatedSegments.Add(CreateTranslatedSegment(segments, segmentIndex));
						}
						else
						{
							mtCloudSegments.Add(CreateMTCloudSegments(segments, segmentIndex));
						}
					}
					else
					{
						mtCloudSegments.Add(CreateMTCloudSegments(segments, segmentIndex));
					}
				}

				if (mtCloudSegments.Count > 0)
				{
					var hasTranslations = GetTranslations(mtCloudSegments);
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
				var translations = TranslateSegments(segments.ToArray());

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

		private bool GetTranslations(IReadOnlyList<MTCloudSegment> mtCloudSegments)
		{
			var translations = TranslateSegments(mtCloudSegments.Select(segment => segment.Segment).ToArray());

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

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits,
			bool[] mask)
		{
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

		private bool IsSameSourceTarget(TranslationUnit corespondingTu)
		{
			if (corespondingTu.TargetSegment == null || corespondingTu.SourceSegment == null)
			{
				return false;
			}

			return corespondingTu.SourceSegment.ToString().Equals(corespondingTu.TargetSegment.ToString());
		}
	}
}
