using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicrosoftTranslatorProvider
{
	public class TranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly TranslationProvider _translationProvider;
		private readonly ITranslationOptions _translationOptions;
		private readonly LanguagePair _languagePair;

		private TranslationUnit _currentTranslationUnit;
		private MicrosoftSegmentEditor _preLookupSegmentEditor;
		private MicrosoftSegmentEditor _postLookupSegmentEditor;

		public TranslationProviderLanguageDirection(TranslationProvider provider, ITranslationOptions translationOptions, LanguagePair languagePair)
		{
			_translationProvider = provider;
			_translationOptions = translationOptions;
			_languagePair = languagePair;
		}

		public ITranslationProvider TranslationProvider => _translationProvider;

		public bool CanReverseLanguageDirection => false;

		CultureCode ITranslationProviderLanguageDirection.SourceLanguage => _languagePair.SourceCulture;

		CultureCode ITranslationProviderLanguageDirection.TargetLanguage => _languagePair.TargetCulture;

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			var searchResults = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; i++)
			{
				searchResults[i] = SearchSegment(settings, segments[i]);
			}

			return searchResults;
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			var translation = new Segment(_languagePair.TargetCulture);
			var searchResults = new SearchResults { SourceSegment = segment.Duplicate() };
			if (!_translationOptions.ProviderSettings.ResendDrafts && _currentTranslationUnit.ConfirmationLevel != ConfirmationLevel.Unspecified)
			{
				return searchResults;
			}

			var newSegment = segment.Duplicate();
			if (!_translationOptions.ProviderSettings.IncludeTags || !newSegment.HasTags)
			{
				translation.Add(SearchSegmentOnTextOnly(newSegment));
				searchResults.Add(CreateSearchResult(newSegment, translation));
				return searchResults;
			}

			if (_translationOptions.ProviderSettings.UsePreLookup)
			{
				_preLookupSegmentEditor ??= new MicrosoftSegmentEditor(_translationOptions.ProviderSettings.PreLookupFilePath);
				newSegment = GetEditedSegment(_preLookupSegmentEditor, newSegment);
			}

			var tagPlacer = new TagPlacer(newSegment);
			var translatedText = Lookup(tagPlacer.PreparedSourceText);
			translation = tagPlacer.GetTaggedSegment(translatedText).Duplicate();
			if (_translationOptions.ProviderSettings.UsePostLookup)
			{
				_postLookupSegmentEditor ??= new MicrosoftSegmentEditor(_translationOptions.ProviderSettings.PostLookupFilePath);
				translation = GetEditedSegment(_postLookupSegmentEditor, translation);
			}

			searchResults.Add(CreateSearchResult(newSegment, translation));
			return searchResults;
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			if (segments is null || mask is null)
			{
				throw new ArgumentNullException("null in SearchSegmentsMasked");
			}
			else if (mask.Length != segments.Length)
			{
				throw new ArgumentException("length SearchSegmentsMasked");
			}

			var results = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; ++i)
			{
				if (mask[i])
				{
					results[i] = SearchSegment(settings, segments[i]);
					continue;
				}

				results[i] = null;
			}

			return results;
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			var currentSegment = new Segment(_languagePair.SourceCulture);
			currentSegment.Add(segment);
			return SearchSegment(settings, currentSegment);
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			_currentTranslationUnit = translationUnit;
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			var results = new SearchResults[translationUnits.Length];
			for (var i = 0; i < translationUnits.Length; ++i)
			{
				if (translationUnits[i] is null)
				{
					continue;
				}

				_currentTranslationUnit = translationUnits[i];
				results[i] = SearchSegment(settings, translationUnits[i].SourceSegment);
			}

			return results;
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			var results = new List<SearchResults>(mask.Length);
			for (var i = 0; i < translationUnits.Length; i++)
			{
				if (mask[i])
				{
					results.Add(SearchTranslationUnit(settings, translationUnits[i]));
					continue;
				}

				results.Add(null);
			}

			return results.ToArray();
		}

		private string SearchSegmentOnTextOnly(Segment segment)
		{
			var sourcetext = segment.ToPlain();
			if (_translationOptions.ProviderSettings.UsePreLookup)
			{
				_preLookupSegmentEditor ??= new MicrosoftSegmentEditor(_translationOptions.ProviderSettings.PreLookupFilePath);
				sourcetext = GetEditedString(_preLookupSegmentEditor, sourcetext);
				segment.Clear();
				segment.Add(sourcetext);
			}

			var translatedText = Lookup(sourcetext);
			if (_translationOptions.ProviderSettings.UsePostLookup)
			{
				_postLookupSegmentEditor ??= new MicrosoftSegmentEditor(_translationOptions.ProviderSettings.PostLookupFilePath);
				translatedText = GetEditedString(_postLookupSegmentEditor, translatedText);
			}

			return translatedText;
		}

		private SearchResult CreateSearchResult(Segment searchSegment, Segment translation)
		{
			var translationUnit = new TranslationUnit
			{
				SourceSegment = searchSegment.Duplicate(),
				TargetSegment = translation,
				Origin = TranslationUnitOrigin.Nmt
			};

			translationUnit.ResourceId = new PersistentObjectToken(translationUnit.GetHashCode(), Guid.Empty);
			var searchResult = new SearchResult(translationUnit) { ScoringResult = new ScoringResult { BaseScore = 0 } };
			translationUnit.ConfirmationLevel = ConfirmationLevel.Draft;
			searchResult.TranslationProposal = new TranslationUnit(translationUnit);

			return searchResult;
		}

		private string GetEditedString(MicrosoftSegmentEditor editor, string sourcetext)
		{
			return editor.EditText(sourcetext);
		}

		private Segment GetEditedSegment(MicrosoftSegmentEditor editor, Segment inSegment)
		{
			var newSegment = new Segment(inSegment.Culture);
			foreach (var element in inSegment.Elements)
			{
				if (element.GetType() == typeof(Tag))
				{
					newSegment.Add(element);
					continue;
				}

				var temp = editor.EditText(element.ToString());
				newSegment.Add(temp);
			}

			return newSegment;
		}

		private string Lookup(string sourcetext)
		{
			var sourcelang = _languagePair.SourceCultureName.ToString();
			var targetlang = _languagePair.TargetCultureName.ToString();

			var pairMapped = _translationOptions.PairModels.FirstOrDefault(x => x.TradosLanguagePair.SourceCultureName == _languagePair.SourceCultureName && x.TradosLanguagePair.TargetCultureName == _languagePair.TargetCultureName);
			var translation = _translationOptions.AuthenticationType switch
			{
				AuthenticationType.Microsoft => MicrosoftService.TranslateAsync(pairMapped, sourcetext, _translationOptions.MicrosoftCredentials).Result,
				AuthenticationType.PrivateEndpoint => PrivateEndpointService.Translate(_translationOptions.PrivateEndpoint, _translationOptions.ProxySettings, pairMapped, sourcetext)
			};

			return translation;
		}


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