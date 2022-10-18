using System;
using System.Collections.Generic;
using MTEnhancedMicrosoftProvider.Service;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;
using MTEnhancedMicrosoftProvider.Interfaces;
using MTEnhancedMicrosoftProvider.Helpers;
using MTEnhancedMicrosoftProvider.Model;

namespace MTEnhancedMicrosoftProvider
{
	public class ProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
		private readonly LanguagePair _languageDirection;
		private readonly ITranslationOptions _options;
		private readonly Provider _provider;
		private readonly HtmlUtil _htmlUtil;
		private ProviderConnecter _mstConnect;
		private MTESegmentEditor _postLookupSegmentEditor;
		private MTESegmentEditor _preLookupSegmentEditor;


		public ProviderLanguageDirection(Provider provider, LanguagePair languagePair, HtmlUtil htmlUtil)
		{
			_provider = provider;
			_options = _provider.Options;
			_languageDirection = languagePair;
			_htmlUtil = htmlUtil;
		}


		public ITranslationProvider TranslationProvider => _provider;

		public bool CanReverseLanguageDirection => false;

		public System.Globalization.CultureInfo SourceLanguage => _languageDirection.SourceCulture;

		public System.Globalization.CultureInfo TargetLanguage => _languageDirection.TargetCulture;


		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
			var output = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; i++)
			{
				output[i] = SearchSegment(settings, segments[i]);
			}

			return output;
        }

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
			var results = new SearchResults
			{
				SourceSegment = segment.Duplicate()
			};

			var newSegment = segment.Duplicate();
			var translation = new Segment(_languageDirection.TargetCulture);
			var sendTextOnly = _options.SendPlainTextOnly || !newSegment.HasTags;
			if (!sendTextOnly)
			{
				if (_options.UsePreEdit)
				{
					if (_preLookupSegmentEditor is null)
					{
						_preLookupSegmentEditor = new MTESegmentEditor(_options.PreLookupFilename);
					}

					newSegment = GetEditedSegment(_preLookupSegmentEditor, newSegment);
				}

				var tagplacer = new TagPlacer(newSegment, _htmlUtil);
				////tagplacer is constructed and gives us back a properly marked up source string for google
				var translatedText = Lookup(tagplacer.PreparedSourceText, _options);
				//now we send the output back to tagplacer for our properly tagged segment
				translation = tagplacer.GetTaggedSegment(translatedText).Duplicate();
				if (_options.UsePostEdit)
				{
					if (_postLookupSegmentEditor is null)
					{
						_postLookupSegmentEditor = new MTESegmentEditor(_options.PostLookupFilename);
					}

					translation = GetEditedSegment(_postLookupSegmentEditor, translation);
				}
			}
			else
			{
				var sourcetext = newSegment.ToPlain();
				if (_options.UsePreEdit)
				{
					if (_preLookupSegmentEditor is null)
					{
						_preLookupSegmentEditor = new MTESegmentEditor(_options.PreLookupFilename);
					}

					sourcetext = GetEditedString(_preLookupSegmentEditor, sourcetext);
					newSegment.Clear();
					newSegment.Add(sourcetext);
				}

				var translatedText = Lookup(sourcetext, _options);
				if (_options.UsePostEdit)
				{
					if (_postLookupSegmentEditor is null)
					{
						_postLookupSegmentEditor = new MTESegmentEditor(_options.PostLookupFilename);
					}

					translatedText = GetEditedString(_postLookupSegmentEditor, translatedText);
				}

				translation.Add(translatedText);
			}

			results.Add(CreateSearchResult(newSegment, translation));
			return results;
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
			var currentSegment = new Segment(_languageDirection.SourceCulture);
			currentSegment.Add(segment);
			return SearchSegment(settings, currentSegment);
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			var results = new SearchResults[translationUnits.Length];
			for (var p = 0; p < translationUnits.Length; ++p)
			{
				if (translationUnits[p] == null)
				{
					continue;
				}

				results[p] = SearchSegment(settings, translationUnits[p].SourceSegment);
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


		private SearchResult CreateSearchResult(Segment searchSegment, Segment translation)
		{
			var translationUnit = new TranslationUnit
			{
				SourceSegment = searchSegment.Duplicate(),
				TargetSegment = translation,
				Origin = TranslationUnitOrigin.Nmt,
				ConfirmationLevel = ConfirmationLevel.Draft
			};

			translationUnit.ResourceId = new PersistentObjectToken(translationUnit.GetHashCode(), Guid.Empty);
			return new SearchResult(translationUnit)
			{
				ScoringResult = new ScoringResult { BaseScore = 0 },
				TranslationProposal = new TranslationUnit(translationUnit)
			};
		}

		private string GetEditedString(Model.MTESegmentEditor editor, string sourcetext)
		{
			return editor.EditText(sourcetext);
		}

		private Segment GetEditedSegment(Model.MTESegmentEditor editor, Segment inSegment)
		{
			var newSegment = new Segment(inSegment.Culture);
			foreach (var element in inSegment.Elements)
			{
				if (element.GetType().ToString() == "Sdl.LanguagePlatform.Core.Tag")
				{
					newSegment.Add(element); //if tag just add the tag
					continue;
				}

				var temp = editor.EditText(element.ToString());
				newSegment.Add(temp);
			}

			return newSegment;
		}

		private string Lookup(string sourcetext, ITranslationOptions options)
		{
			var catId = options.UseCatID ? _options.CatId : string.Empty;
			switch (_mstConnect)
			{
				case null:
					_mstConnect = new ProviderConnecter(_options.ClientId, options.Region, _htmlUtil);
					break;
				default:
					_mstConnect.ResetCredentials(options.ClientId, options.Region);
					break;
			}

			var sourcelang = _languageDirection.SourceCulture.ToString();
			var targetlang = _languageDirection.TargetCulture.ToString();
			return _mstConnect.Translate(sourcelang, targetlang, sourcetext, catId);
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