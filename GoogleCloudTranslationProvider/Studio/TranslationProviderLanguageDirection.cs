using System;
using System.Globalization;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.GoogleAPI;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleCloudTranslationProvider.Studio
{
	public class TranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		private readonly ITranslationOptions _translationOptions;
		private readonly LanguagePair _languageDirection;
		private readonly TranslationProvider _provider;
		private readonly HtmlUtil _htmlUtil;

		private V2Connector _googleV2Api;
		private V3Connector _googleV3Api;

		private GoogleSegmentEditor _postLookupSegmentEditor;
		private GoogleSegmentEditor _preLookupSegmentEditor;
		private TranslationUnit _currentTranslationUnit;

		public TranslationProviderLanguageDirection(TranslationProvider provider, LanguagePair languages, HtmlUtil htmlUtil)
		{
			_provider = provider;
			_languageDirection = languages;
			_translationOptions = _provider.Options;
			_htmlUtil = htmlUtil;
		}

		public ITranslationProvider TranslationProvider => _provider;

		public bool CanReverseLanguageDirection => false;

		public CultureCode SourceLanguage => _languageDirection.SourceCulture;

		public CultureCode TargetLanguage => _languageDirection.TargetCulture;

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
			DatabaseExtensions.CreateDatabase(_translationOptions);
			var translation = new Segment(_languageDirection.TargetCulture);
			var searchResults = new SearchResults { SourceSegment = segment.Duplicate() };
			if (!_translationOptions.ResendDrafts && _currentTranslationUnit.ConfirmationLevel != ConfirmationLevel.Unspecified)
			{
				translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);
				searchResults.Add(CreateSearchResult(segment, translation));
				return searchResults;
			}

			var newSegment = segment.Duplicate();
			if (_translationOptions.SendPlainTextOnly || !newSegment.HasTags)
			{
				translation.Add(SearchSegmentOnTextOnly(newSegment));
				searchResults.Add(CreateSearchResult(newSegment, translation));
				return searchResults;
			}

			if (_translationOptions.UsePreEdit)
			{
				_preLookupSegmentEditor ??= new GoogleSegmentEditor(_translationOptions.PreLookupFilename);
				newSegment = GetEditedSegment(_preLookupSegmentEditor, newSegment);
			}

			var tagplacer = new TagPlacer(newSegment, _htmlUtil);
			var translatedText = Lookup(tagplacer.PreparedSourceText, _translationOptions, "html");
			translation = tagplacer.GetTaggedSegment(translatedText).Duplicate();
			if (_translationOptions.UsePostEdit)
			{
				_postLookupSegmentEditor ??= new GoogleSegmentEditor(_translationOptions.PostLookupFilename);
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
				throw new ArgumentException("length in SearchSegmentsMasked");
			}

			var searchResults = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; ++i)
			{
				searchResults[i] = mask[i] ? SearchSegment(settings, segments[i])
										   : null;
			}

			return searchResults;
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			var currentSegment = new Segment(_languageDirection.SourceCulture);
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
			var searchResults = new SearchResults[translationUnits.Length];
			for (var i = 0; i < translationUnits.Length; ++i)
			{
				if (translationUnits[i] is null)
				{
					continue;
				}

				_currentTranslationUnit = translationUnits[i];
				searchResults[i] = SearchSegment(settings, translationUnits[i].SourceSegment);
			}

			return searchResults;
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

		private string SearchSegmentOnTextOnly(Segment segment)
		{
			var sourcetext = segment.ToPlain();
			if (_translationOptions.UsePreEdit)
			{
				_preLookupSegmentEditor ??= new GoogleSegmentEditor(_translationOptions.PreLookupFilename);
				sourcetext = GetEditedString(_preLookupSegmentEditor, sourcetext);
				segment.Clear();
				segment.Add(sourcetext);
			}

			var translatedText = Lookup(sourcetext, _translationOptions, "text");
			if (_translationOptions.UsePostEdit)
			{
				_postLookupSegmentEditor ??= new GoogleSegmentEditor(_translationOptions.PostLookupFilename);
				translatedText = GetEditedString(_postLookupSegmentEditor, translatedText);
			}

			return translatedText;
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

		private string GetEditedString(GoogleSegmentEditor editor, string sourcetext)
		{
			return editor.EditText(sourcetext);
		}

		private Segment GetEditedSegment(GoogleSegmentEditor editor, Segment inSegment)
		{
			var segment = new Segment(inSegment.Culture);
			foreach (var element in inSegment.Elements)
			{
				if (element.GetType() == typeof(Tag))
				{
					segment.Add(element);
					continue;
				}

				segment.Add(editor.EditText(element.ToString()));
			}

			return segment;
		}

		private string Lookup(string sourcetext, ITranslationOptions options, string format)
		{
			if (options.SelectedGoogleVersion is ApiVersion.V3)
			{
				_googleV3Api = new V3Connector(options);
				return _googleV3Api.TranslateText(_languageDirection.SourceCulture, _languageDirection.TargetCulture, sourcetext, format);
			}

			if (_googleV2Api is null)
			{
				_googleV2Api = new V2Connector(options.ApiKey, _htmlUtil);
			}
			else
			{
				_googleV2Api.ApiKey = options.ApiKey;
			}

			return _googleV2Api.Translate(_languageDirection, sourcetext, format);
		}


		#region Unused
		/// <summary>
		/// Not required for this implementation.
		/// </summary>
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation.
		/// </summary>
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			return new ImportResult[] { AddTranslationUnit(translationUnits[translationUnits.GetLength(0) - 1], settings) };
		}

		/// <summary>
		/// Not required for this implementation.
		/// </summary>
		/// <param name="translationUnit"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation.
		/// </summary>
		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation.
		/// </summary>
		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation.
		/// </summary>
		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not required for this implementation.
		/// </summary>
		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}