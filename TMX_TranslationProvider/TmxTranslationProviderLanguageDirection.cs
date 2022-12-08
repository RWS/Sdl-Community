﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace TMX_TranslationProvider
{
	public class TmxTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		#region ITranslationProviderLanguageDirection Members

		private LanguagePair _languagePair;
		private TmxTranslationProvider _provider;

		public LanguagePair LanguagePair => _languagePair;
		public System.Globalization.CultureInfo SourceLanguage => _languagePair.SourceCulture;
		public System.Globalization.CultureInfo TargetLanguage => _languagePair.TargetCulture;

		public ITranslationProvider TranslationProvider => _provider;
		public bool CanReverseLanguageDirection => false;

		private ProjectsController _projectsController;


		public TmxTranslationProviderLanguageDirection(LanguagePair languagePair, TmxTranslationProvider provider)
		{
			_languagePair = languagePair;
			_provider = provider;

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
		}


		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			var searcher = _provider.SearchService;
			if (searcher == null)
				return new SearchResults();
			var task = searcher.Search(settings, segment, _languagePair);
			task.Wait();
			return task.Result;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			var results = new SearchResults[segments.Length];
			for (var p = 0; p < segments.Length; ++p)
			{
				results[p] = SearchSegment(settings, segments[p]);
			}
			return results;
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			if (segments == null)
				throw new ArgumentNullException(@"segments in SearchSegmentsMasked");
			if (mask == null || mask.Length != segments.Length)
				throw new ArgumentException(@"mask in SearchSegmentsMasked");

			var results = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; ++i)
				results[i] = mask[i] ? SearchSegment(settings, segments[i]) : null;

			return results;
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			var s = new Segment(_languagePair.SourceCulture);
			s.Add(segment);
			return SearchSegment(settings, s);
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			var searchResult = new SearchResults();
			if (translationUnit.TargetSegment !=null)
            {
                if (!_provider.Options.IgnoreTranslatedSegments )
                {
                    searchResult = SearchSegment(settings, translationUnit.SourceSegment);
                }
                else
                {
                    if (translationUnit.TargetSegment.IsEmpty)
                        searchResult = SearchSegment(settings, translationUnit.SourceSegment);
                    else
                        searchResult.SourceSegment = translationUnit.SourceSegment.Duplicate();
                }                
            }
            else
                searchResult = SearchSegment(settings, translationUnit.SourceSegment);
            
            
            return searchResult;
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			var results = new SearchResults[translationUnits.Length];
			for (var p = 0; p < translationUnits.Length; ++p)
			{
				results[p] = SearchSegment(settings, translationUnits[p].SourceSegment);
			}
			return results;
		}

		private SearchResults EmptyTU()
		{
			var result = new SearchResults();
			var source = new Segment(SourceLanguage);
			var target = new Segment(TargetLanguage);
			result.SourceSegment = source;
			var tu = new TranslationUnit
			{
				SourceSegment = source,
				TargetSegment = target,
				ConfirmationLevel = ConfirmationLevel.Draft,
			};

			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
			tu.Origin = TranslationUnitOrigin.TM;

			var sr = new SearchResult(tu)
			{
				ScoringResult = new ScoringResult
				{
					BaseScore = 0,
				},
				TranslationProposal = tu,
			};
			result.Add(sr);
			return result;
		}
		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			var sr = SearchTranslationUnits(settings, translationUnits);
			return sr;
		}


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