using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.MtEnhancedProvider.MstConnect;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider
{
    public class MtTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        #region "PrivateMembers"
        private readonly MtTranslationProvider _provider;
        private readonly LanguagePair _languageDirection;
        private readonly MtTranslationOptions _options;
        private  TranslationUnit _inputTu;
        private MtTranslationProviderGTApiConnecter _gtConnect;
        private ApiConnecter _mstConnect;
        private SegmentEditor _postLookupSegmentEditor;
        private SegmentEditor _preLookupSegmentEditor;
        #endregion

        #region "ITranslationProviderLanguageDirection Members"

        /// <summary>
        /// Instantiates the variables and fills the list file content into
        /// a Dictionary collection object.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="languages"></param>
        #region "ListTranslationProviderLanguageDirection"
        public MtTranslationProviderLanguageDirection(MtTranslationProvider provider, LanguagePair languages)
        {
            #region "Instantiate"
            _provider = provider;
            _languageDirection = languages;
            _options = _provider.Options;
            #endregion
        }
        #endregion

        public System.Globalization.CultureInfo SourceLanguage => _languageDirection.SourceCulture;

        public System.Globalization.CultureInfo TargetLanguage => _languageDirection.TargetCulture;

        public ITranslationProvider TranslationProvider => _provider;

        private string LookupGt(string sourcetext, MtTranslationOptions options, string format)
        {
            //instantiate GtApiConnecter if necessary
            if (_gtConnect == null)
            {
                // need to get and insert key
                _gtConnect = new MtTranslationProviderGTApiConnecter(options.ApiKey); //needs key
            }
            else
            {
                _gtConnect.ApiKey = options.ApiKey; //reset key in case it has been changed in dialog since GtApiConnecter was instantiated
            }
            var translatedText = _gtConnect.Translate(_languageDirection, sourcetext, format);

            return translatedText;
        }

        private string LookupMst(string sourcetext, MtTranslationOptions options, string format)
        {
            var catId = "";
            if (options.UseCatID)
                catId = _options.CatId;//only use specific category ID if the option is selected
            var sourcelang = _languageDirection.SourceCulture.ToString();
            var targetlang = _languageDirection.TargetCulture.ToString();

            //instantiate ApiConnecter if necessary
            if (_mstConnect == null)
            {
                _mstConnect = new ApiConnecter(_options);
            }
            else
            {
                _mstConnect.resetCrd(options.ClientId, options.ClientSecret); //reset key in case it has been changed in dialog since GtApiConnecter was instantiated
            }

            var translatedText = _mstConnect.Translate(sourcelang, targetlang, sourcetext, catId, format);
            return translatedText;
        }
		
        /// <summary>
        /// Performs the actual search by looping through the
        /// delimited segment pairs contained in the text file.
        /// Depening on the search mode, a segment lookup (with exact machting) or a source / target
        /// concordance search is done.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        #region "SearchSegment"
        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            var translation = new Segment(_languageDirection.TargetCulture);//this will be the target segment

            #region "SearchResultsObject"
            var results = new SearchResults();
            results.SourceSegment = segment.Duplicate();
            #endregion

            #region "Confirmation Level"
            if (!_options.ResendDrafts && _inputTu.ConfirmationLevel != ConfirmationLevel.Unspecified) //i.e. if it's status is other than untranslated
            { //don't do the lookup, b/c we don't need to pay google to translate text already translated if we edit a segment
                translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);
                //later get these strings from resource file
                results.Add(CreateSearchResult(segment, translation, segment.ToString()));
                return results;
            }
            #endregion
            // Look up the currently selected segment in the collection (normal segment lookup).
            #region "SegmentLookup"
            var translatedText = "";
            //a new seg avoids modifying the current segment object
            var newseg = segment.Duplicate(); 
        
            //do preedit if checked
            var sendTextOnly = _options.SendPlainTextOnly || !newseg.HasTags;
            if (!sendTextOnly)
            {
                //do preedit with tagged segment
                if (_options.UsePreEdit)
                {
                    if (_preLookupSegmentEditor == null) _preLookupSegmentEditor = new SegmentEditor(_options.PreLookupFilename);
                    newseg = GetEditedSegment(_preLookupSegmentEditor, newseg);
                }
                //return our tagged target segment
                var tagplacer = new MtTranslationProviderTagPlacer(newseg);
                ////tagplacer is constructed and gives us back a properly marked up source string for google
                if (_options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
                {
                    translatedText = LookupGt(tagplacer.PreparedSourceText, _options, "html");
                }
                else if (_options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
                {
                    translatedText = LookupMst(tagplacer.PreparedSourceText, _options, "text/html");
                }
                //now we send the output back to tagplacer for our properly tagged segment
                translation = tagplacer.GetTaggedSegment(translatedText).Duplicate();

                //now do post-edit if that option is checked
                if (_options.UsePostEdit)
                {
                    if (_postLookupSegmentEditor == null) _postLookupSegmentEditor = new SegmentEditor(_options.PostLookupFilename);
                    translation = GetEditedSegment(_postLookupSegmentEditor, translation);
                }
            }
            else //only send plain text
            {
                var sourcetext = newseg.ToPlain();
                //do preedit with string
                if (_options.UsePreEdit)
                {
                    if (_preLookupSegmentEditor == null) _preLookupSegmentEditor = new SegmentEditor(_options.PreLookupFilename);
                    sourcetext = GetEditedString(_preLookupSegmentEditor, sourcetext);
                    //change our source segment so it gets sent back with modified text to show in translation results window that it was changed before sending
                    newseg.Clear();
                    newseg.Add(sourcetext);
                }

                //now do lookup
                if (_options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
                {
                    translatedText = LookupGt(sourcetext, _options, "html"); //plain??
                }
                else if (_options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
                {
                    translatedText = LookupMst(sourcetext, _options, "text/plain");
                }
                //now do post-edit if that option is checked
                if (_options.UsePostEdit)
                {
                    if (_postLookupSegmentEditor == null) _postLookupSegmentEditor = new SegmentEditor(_options.PostLookupFilename);
                    translatedText = GetEditedString(_postLookupSegmentEditor, translatedText);
                }
                translation.Add(translatedText);
            }

            results.Add(CreateSearchResult(newseg, translation, newseg.ToPlain()));

            #endregion

            #region "Close"
            return results;
            #endregion
        }
        #endregion

        /// <summary>
        /// Used to do batch find-replace on a segment with tags.
        /// </summary>
        /// <param name="inSegment"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Segment GetEditedSegment(SegmentEditor editor, Segment inSegment)
        {
            
            var newSeg = new Segment(inSegment.Culture);

            foreach (var element in inSegment.Elements)
            {
                var elType = element.GetType();

                if (elType.ToString() != "Sdl.LanguagePlatform.Core.Tag") //if other than tag, make string and edit it
                {
                    var temp = editor.EditText(element.ToString());
                    newSeg.Add(temp); //add edited text to segment
                }
                else
                {
                    newSeg.Add(element); //if tag just add the tag
                }
            }
        return newSeg;
        }

        /// <summary>
        /// Used to do batch find-replace on a string of plain text.
        /// </summary>
        /// <param name="sourcetext"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetEditedString(SegmentEditor editor, string sourcetext)
        {
            var result = editor.EditText(sourcetext);
            return result;
        }
        /// <summary>
        /// Creates the translation unit as it is later shown in the Translation Results
        /// window of SDL Trados Studio. This member also determines the match score
        /// (in our implementation always 100%, as only exact matches are supported)
        /// as well as the confirmation level, i.e. Translated.
        /// </summary>
        /// <param name="searchSegment"></param>
        /// <param name="translation"></param>
        /// <param name="sourceSegment"></param>
        /// <returns></returns>
        #region "CreateSearchResult"
        private SearchResult CreateSearchResult(Segment searchSegment, Segment translation,
            string sourceSegment)
        {
            #region "TranslationUnit"
            var tu = new TranslationUnit();
            tu.SourceSegment = searchSegment.Duplicate();//this makes the original source segment, with tags, appear in the search window
            tu.TargetSegment = translation;
            #endregion

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            var score = 0; //score to 0...change if needed to support scoring
            tu.Origin = TranslationUnitOrigin.MachineTranslation;
            var searchResult = new SearchResult(tu);
            searchResult.ScoringResult = new ScoringResult();
            searchResult.ScoringResult.BaseScore = score;
            tu.ConfirmationLevel = ConfirmationLevel.Draft;

            return searchResult;
        }
        #endregion

        public bool CanReverseLanguageDirection { get; } = false;

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
            {
                throw new ArgumentNullException("segments in SearchSegmentsMasked");
            }
            if (mask == null || mask.Length != segments.Length)
            {
                throw new ArgumentException("mask in SearchSegmentsMasked");
            }

            var results = new SearchResults[segments.Length];
            for (var p = 0; p < segments.Length; ++p)
            {
                if (mask[p])
                {
                    results[p] = SearchSegment(settings, segments[p]);
                }
                else
                {
                    results[p] = null;
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
            //need to use the tu confirmation level in searchsegment method
            _inputTu = translationUnit;
            return SearchSegment(settings, translationUnit.SourceSegment);
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            var results = new SearchResults[translationUnits.Length];
            for (var p = 0; p < translationUnits.Length; ++p)
            {
                //need to use the tu confirmation level in searchsegment method
                _inputTu = translationUnits[p];
                results[p] = SearchSegment(settings, translationUnits[p].SourceSegment); //changed this to send whole tu
            }
            return results;
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
	        // because of the following bug LG-15128 where mask parameters are true for both CM and the actual TU to be updated which cause an unnecessary call for CM segment
	        //we take only the last translation unit because the first one is CM
	        //this workaround works only when LookAhead option is disabled from Studio
	        var results = new List<SearchResults>();
	        if (!mask.Any(m => m.Equals(false)) && mask.Length > 1)
	        {
		        var lastTu = translationUnits[translationUnits.Length - 1];
		        var result = SearchTranslationUnit(settings, lastTu);
		        results.Add(null);
		        results.Add(result);
	        }
	        else
	        {
		        var i = 0;
		        foreach (var tu in translationUnits)
		        {
			        if (mask == null || mask[i])
			        {
				        var result = SearchTranslationUnit(settings, tu);
				        results.Add(result);
			        }
			        else
			        {
				        results.Add(null);
			        }
			        i++;
		        }
	        }
	        return results.ToArray();
		}

        #region "NotForThisImplementation"
        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="settings"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnit"></param>
        /// <returns></returns>
        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {

            throw new NotImplementedException();

        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <returns></returns>
        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="previousTranslationHashes"></param>
        /// <param name="settings"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            ImportResult[] result = { AddTranslationUnit(translationUnits[translationUnits.GetLength(0) - 1], settings) };
            return result;
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
        /// <param name="translationUnits"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="previousTranslationHashes"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}
