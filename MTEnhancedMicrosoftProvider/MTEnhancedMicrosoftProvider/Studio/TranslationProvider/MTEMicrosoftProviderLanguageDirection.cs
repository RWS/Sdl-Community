using System;
using System.Collections.Generic;
using MTEnhancedMicrosoftProvider.Connect;
using MTEnhancedMicrosoftProvider.Service;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;
using MTEnhancedMicrosoftProvider.Interfaces;

namespace MTEnhancedMicrosoftProvider
{
	public class MTEMicrosoftProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
		private readonly LanguagePair _languageDirection;
		private readonly ITranslationOptions _options;
		private readonly MTEMicrosoftProvider _provider;
		private readonly HtmlUtil _htmlUtil;
		private TranslationUnit _inputTu;
		private ApiConnecter _mstConnect;
		private Model.SegmentEditor _postLookupSegmentEditor;
		private Model.SegmentEditor _preLookupSegmentEditor;


		public MTEMicrosoftProviderLanguageDirection(MTEMicrosoftProvider provider, LanguagePair languagePair, HtmlUtil htmlUtil)
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
			for (int i = 0; i < segments.Length; i++)
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

			var newseg = segment.Duplicate();
			var translation = new Segment(_languageDirection.TargetCulture);
			if (!_options.ResendDrafts
				&& _inputTu.ConfirmationLevel != ConfirmationLevel.Unspecified)
			{
				translation.Add(Lookup(newseg.ToPlain(), _options, "text/plain"));
				results.Add(CreateSearchResult(segment, translation));
				return results;
			}

			var sendTextOnly = _options.SendPlainTextOnly || !newseg.HasTags;
			if (!sendTextOnly)
			{
				if (_options.UsePreEdit)
				{
					if (_preLookupSegmentEditor == null)
					{
						_preLookupSegmentEditor = new Model.SegmentEditor(_options.PreLookupFilename);
					}

					newseg = GetEditedSegment(_preLookupSegmentEditor, newseg);
				}

				var tagplacer = new TagPlacer(newseg, _htmlUtil);
				////tagplacer is constructed and gives us back a properly marked up source string for google
				var translatedText = Lookup(tagplacer.PreparedSourceText, _options, "text/html");
				//now we send the output back to tagplacer for our properly tagged segment
				translation = tagplacer.GetTaggedSegment(translatedText).Duplicate();
				if (_options.UsePostEdit)
				{
					if (_postLookupSegmentEditor == null)
					{
						_postLookupSegmentEditor = new Model.SegmentEditor(_options.PostLookupFilename);
					}

					translation = GetEditedSegment(_postLookupSegmentEditor, translation);
				}
			}
			else
			{
				var sourcetext = newseg.ToPlain();
				if (_options.UsePreEdit)
				{
					if (_preLookupSegmentEditor == null)
					{
						_preLookupSegmentEditor = new Model.SegmentEditor(_options.PreLookupFilename);
					}

					sourcetext = GetEditedString(_preLookupSegmentEditor, sourcetext);
					newseg.Clear();
					newseg.Add(sourcetext);
				}

				var translatedText = Lookup(sourcetext, _options, "text/plain");
				if (_options.UsePostEdit)
				{
					if (_postLookupSegmentEditor == null)
					{
						_postLookupSegmentEditor = new Model.SegmentEditor(_options.PostLookupFilename);
					}

					translatedText = GetEditedString(_postLookupSegmentEditor, translatedText);
				}

				translation.Add(translatedText);
			}

			results.Add(CreateSearchResult(newseg, translation));

			return results;
		}

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
			if (segments == null)
			{
				throw new ArgumentNullException("segments in SearchSegmentsMasked");
			}
			else if (mask == null || mask.Length != segments.Length)
			{
				throw new ArgumentException("mask in SearchSegmentsMasked");
			}

			var results = new SearchResults[segments.Length];
			for (var i = 0; i < segments.Length; ++i)
			{
				if (mask[i])
				{
					results[i] = SearchSegment(settings, segments[i]);
				}
				else
				{
					results[i] = null;
				}
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
			_inputTu = translationUnit;
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

				_inputTu = translationUnits[p];
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
				}
				else
				{
					results.Add(null);
				}
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

		private string GetEditedString(Model.SegmentEditor editor, string sourcetext)
		{
			return editor.EditText(sourcetext);
		}

		private Segment GetEditedSegment(Model.SegmentEditor editor, Segment inSegment)
		{
			var newSeg = new Segment(inSegment.Culture);
			foreach (var element in inSegment.Elements)
			{
				var elType = element.GetType();
				if (elType.ToString() == "Sdl.LanguagePlatform.Core.Tag")
				{
					newSeg.Add(element); //if tag just add the tag
					continue;
				}

				var temp = editor.EditText(element.ToString());
				newSeg.Add(temp);
			}

			return newSeg;
		}

		private string Lookup(string sourcetext, ITranslationOptions options, string format)
		{
			var catId = "";
			if (options.UseCatID)
			{
				catId = _options.CatId;
			}

			if (_mstConnect == null)
			{
				_mstConnect = new ApiConnecter(_options.ClientId, options.Region, _htmlUtil);
			}
			else
			{
				_mstConnect.ResetCredentials(options.ClientId, options.Region);
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