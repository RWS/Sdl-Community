using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Segment = Sdl.LanguagePlatform.Core.Segment;


namespace Sdl.Community.Taus.Translation.Provider
{
    public class TausTranslationProviderLanguageDirection :  ITranslationProviderLanguageDirection
    {
        #region "PrivateMembers"
        private readonly TausTranslationProvider _provider;
        private readonly LanguagePair _languageDirection;
        private readonly TausTranslationOptions _options;
        private readonly TausTranslationProviderElementVisitor _visitor;

 
        #endregion

        #region "ITranslationProviderLanguageDirection Members"


        public TausTranslationProviderLanguageDirection(TausTranslationProvider provider, LanguagePair languages)
        {
            
            _provider = provider;
            _languageDirection = languages;
            _options = _provider.Options;
            _visitor = new TausTranslationProviderElementVisitor(_options);

            

            

            //#region "CompileCollection"

            //_listOfTranslations = new Dictionary<string, string>();

            //// Load the content of the specified list file and fill it
            //// into the multiple identical sources are not allowed
            //using (StreamReader sourceFile = new StreamReader(_options.ListFileName))
            //{   
            //    sourceFile.ReadLine(); // Skip the first line as it contains the language direction.

            //    char fileDelimiter = Convert.ToChar(_options.Delimiter);
            //    while (!sourceFile.EndOfStream)
            //    {
            //        string[] currentPair = sourceFile.ReadLine().Split(fileDelimiter);
            //        if (currentPair.Count<string>() != 2)
            //        { 
            //            // The current line does not contain a proper source/target segment pair.
            //            continue;
            //        }

            //        // Add the source/target segment pair to the collection
            //        // after checking that the current source segment does not
            //        // already exist in the Dictionary.
            //        if (!_listOfTranslations.ContainsKey(currentPair[0]))
            //        { 
            //            _listOfTranslations.Add(currentPair[0] ,currentPair[1]);
            //        }
            //    }
            //    sourceFile.Close();
            //}
            //#endregion
        }


        public System.Globalization.CultureInfo SourceLanguage
        {
            get { return _languageDirection.SourceCulture; }
        }

        public System.Globalization.CultureInfo TargetLanguage
        {
            get { return _languageDirection.TargetCulture; }
        }

        public ITranslationProvider TranslationProvider
        {
            get { return _provider; }
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
            
            // Loop through segment elements to 'filter out' e.g. tags in order to 
            // make certain that only plain text information is retrieved for
            // this simplified implementation.            
            #region "SegmentElements"
            _visitor.Reset();
            foreach (var element in segment.Elements)
            {
                element.AcceptSegmentElementVisitor(_visitor);
            }
            #endregion

            #region "SearchResultsObject"
            var results = new SearchResults();

            results.SourceSegment = segment.Duplicate();



            #endregion

            
            
            #region  |  Taus.TM.Provider  |


            var tausTmProvider = new Processor();
            var searchSettings = new Sdl.Community.Taus.TM.Provider.Settings.SearchSettings
            {
                Timeout = Convert.ToInt32(_options.SearchTimeout),
                AppKey = _options.ConnectionAppKey,
                UserName = _options.ConnectionUserName,
                Password = _options.ConnectionUserPassword,
                AuthKey = _options.ConnectionAuthKey,
                Limit = settings.MaxResults
            };




            searchSettings.SearchSections.Add(new SegmentSection(true, _visitor.PlainText));

            if (settings.Mode == SearchMode.TargetConcordanceSearch)
            {
                searchSettings.SourceLanguageId = _languageDirection.TargetCultureName;
                searchSettings.TargetLanguageId = _languageDirection.SourceCultureName;
            }
            else
            {
                searchSettings.SourceLanguageId = _languageDirection.SourceCultureName;
                searchSettings.TargetLanguageId = _languageDirection.TargetCultureName;
            }



            searchSettings.IndustryId = Convert.ToInt64(_options.SearchCriteriaIndustryId) > 0 ? _options.SearchCriteriaIndustryId : string.Empty;
            searchSettings.ContentTypeId = Convert.ToInt64(_options.SearchCriteriaContentTypeId) > 0 ? _options.SearchCriteriaContentTypeId : string.Empty;
            searchSettings.ProviderId = Convert.ToInt64(_options.SearchCriteriaProviderId) > 0 ? _options.SearchCriteriaProviderId : string.Empty;
            searchSettings.OwnerId = Convert.ToInt64(_options.SearchCriteriaOwnerId) > 0 ? _options.SearchCriteriaOwnerId : string.Empty;
            searchSettings.ProductId = Convert.ToInt64(_options.SearchCriteriaProductId) > 0 ? _options.SearchCriteriaProductId : string.Empty;


            searchSettings.PenaltySettings.MissingFormattingPenalty = 1;
            searchSettings.PenaltySettings.DifferentFormattingPenalty = 1;
 

            var scoreType = Processor.ScoreType.Concordance;
            if (settings.Mode == SearchMode.NormalSearch)
                scoreType = Processor.ScoreType.Lookup;

            var searchResult = tausTmProvider.SearchSegment(searchSettings, scoreType);

            #endregion


            switch (searchResult.Status)
            {
                case "timed out":
                    //ignore
                    break;
                case "200":
                    foreach (var searchResultSegment in searchResult.Segments)
                    {
                        #region "TargetConcordanceSearch"
                        switch (settings.Mode)
                        {
                            case SearchMode.TargetConcordanceSearch:
                            {
                                var resultSegment = new Sdl.Community.Taus.TM.Provider.Segment.Segment();

                                resultSegment.ContentType = searchResultSegment.ContentType;
                                resultSegment.Id = searchResultSegment.Id;
                                resultSegment.Industry = searchResultSegment.Industry;
                                resultSegment.MatchPercentage = searchResultSegment.MatchPercentage;
                                resultSegment.Owner = searchResultSegment.Owner;
                                resultSegment.Product = searchResultSegment.Product;
                                resultSegment.Provider = searchResultSegment.Provider;

                                resultSegment.SourceLanguage = searchResultSegment.TargetLanguage;
                                resultSegment.TargetLanguage = searchResultSegment.SourceLanguage;


                                resultSegment.SourceSections = searchResultSegment.TargetSections;
                                resultSegment.TargetSections = searchResultSegment.SourceSections;


                                resultSegment.SourceText = searchResultSegment.TargetText;
                                resultSegment.TargetText = searchResultSegment.SourceText;


                                var result = CreateSearchResult(settings, segment, resultSegment, _visitor.PlainText, false);
                                if (result != null)
                                    results.Add(result);

                        
                            }
                                break;
                            case SearchMode.ConcordanceSearch:
                            {
                                var result = CreateSearchResult(settings, segment, searchResultSegment, _visitor.PlainText, false);
                                if (result!=null)
                                    results.Add(result);
                            }
                                break;
                            case SearchMode.NormalSearch:
                            {
                                var result = CreateSearchResult(settings, segment, searchResultSegment, _visitor.PlainText, segment.HasTags);
                                if (result != null)
                                    results.Add(result);
                        
                            }
                                break;
                        }
                        #endregion
                    }
                    break;
                default:
                    throw new Exception(string.Format("Query Exception: Status={0}, Reason={1}", searchResult.Status,
                        searchResult.Reason));
            }


            return results;

        }
        
        
        #endregion

        // a naive tokenizer implementation to treat a single text run as one token.
        private List<Token> Tokenize(Segment segment)
        {
            var tokens = new List<Token>();
            var run = 0;
            foreach (var element in segment.Elements)
            {
                var text = element as Text;
                if (text == null || string.IsNullOrEmpty(text.Value)) continue;
                var token = new global::Sdl.LanguagePlatform.Core.Tokenization.SimpleToken(text.Value)
                {
                    Span = new SegmentRange(run, 0, text.Value.Length - 1)
                };
                tokens.Add(token);
                run++;
            }
            return tokens;
        }

        private IEnumerable<string> GetListOfSimpleWords(string text)
        {
            #region  |  create a list of words  |
            var tmpWords = new List<string>();
            var words = text.Split(' ');

            foreach (var word in words)
            {
                #region  |  word  |

                var wordTmp = string.Empty;

                foreach (var _char in word.ToCharArray())
                {
                    if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2) 
                    {
                        if (wordTmp != string.Empty)
                            tmpWords.Add(wordTmp);
                        wordTmp = string.Empty;

                        tmpWords.Add(_char.ToString());
                    }
                    else
                        wordTmp += _char.ToString();


                }
                if (wordTmp != string.Empty)
                {
                    tmpWords.Add(wordTmp);
                }
                #endregion
            }
            #endregion

            #region  |  get rid of the last punctuation mark at beginning & end  |

            var punctuations = new string[] { "!", ":", ".", ";", "?" };


            //note we don't want to get rid of all punctuation marks
            if (tmpWords.Count > 0)
            {
                var firstWord = tmpWords[0];
                var lastWord = tmpWords[tmpWords.Count - 1];

                //spanish question marks
                if (firstWord.Trim().StartsWith("¿") && lastWord.Trim().EndsWith("?"))
                {
                    tmpWords[0] = tmpWords[0].Substring(tmpWords[0].IndexOf("¿", StringComparison.Ordinal) + 1);
                }


                if (lastWord.Trim().EndsWith("..."))
                {
                    tmpWords[tmpWords.Count - 1] = tmpWords[tmpWords.Count - 1].Substring(0, tmpWords[tmpWords.Count - 1].LastIndexOf("...", StringComparison.Ordinal));
                }
                else
                {
                    var punctuation = lastWord.Trim().Substring(lastWord.Trim().Length - 1);
                    //if (char.IsPunctuation(Convert.ToChar(punctuation)))
                    if (punctuations.Contains(punctuation))
                    {
                        tmpWords[tmpWords.Count - 1] = tmpWords[tmpWords.Count - 1].Substring(0, tmpWords[tmpWords.Count - 1].LastIndexOf(punctuation, StringComparison.Ordinal));
                    }
                }
            }
            #endregion

            #region  |  create unique list of words (ignore case)  |
            var wordList = new List<string>();
            foreach (var word in tmpWords)
            {
                var foundWord = wordList.Any(s => string.Compare(s, word, StringComparison.OrdinalIgnoreCase) == 0);
                if (!foundWord)
                    wordList.Add(word);
            }
            #endregion

            return wordList;
        }
        private List<SegmentRange> CollectConcordanceMatchRanges(Segment segment, string searchString)
        {
            var words = GetListOfSimpleWords(searchString);

            var concordanceMatchRanges = new List<SegmentRange>();


            foreach (var word in words)
            {
                var run = 0;
                var searchLength = word.Length;
                var wordBuilder = string.Empty;
                foreach (var element in segment.Elements)
                {
                    var text = element as Text;
                    if (text == null || string.IsNullOrEmpty(text.Value)) continue;
                    var index = text.Value.IndexOf(word, StringComparison.OrdinalIgnoreCase);

                    while (index >= 0 && index < text.Value.Length)
                    {
                        var segmentRange = new SegmentRange(run, index, index + searchLength - 1);

                        #region  |  test boundry  |
                        var prefixBoundry = true;
                        //test that the beginning is a boundry character
                        if (index > 0)
                        {
                            var c = Convert.ToChar(text.Value.Substring(index - 1, 1));
                            if (!char.IsWhiteSpace(c)
                                && !char.IsPunctuation(c))
                            {
                                prefixBoundry = false;
                            }
                        }
                        else if (wordBuilder != string.Empty)
                        {
                            var c = Convert.ToChar(wordBuilder.Substring(wordBuilder.Length - 1));
                            if (!char.IsWhiteSpace(c)
                                && !char.IsPunctuation(c))
                            {
                                prefixBoundry = false;
                            }
                        }

                        var suffixBountry = true;
                        if (index + searchLength + 1 < text.Value.Length)
                        {
                            var c = Convert.ToChar(text.Value.Substring(index + searchLength, 1));
                            if (!char.IsWhiteSpace(c)
                                && !char.IsPunctuation(c))
                            {
                                suffixBountry = false;
                            }
                        }
                        #endregion
                        if (prefixBoundry && suffixBountry)
                        {
                            concordanceMatchRanges.Add(segmentRange);
                        }

                        index += searchLength;
                        if (index < text.Value.Length)
                            index = text.Value.IndexOf(word, index, StringComparison.OrdinalIgnoreCase);
                    }

                    run++;

                    wordBuilder += text.Value;
                }
            }
            return concordanceMatchRanges;
        }

        /// <summary>
        /// Creates the translation unit as it is later shown in the Translation Results
        /// window of SDL Trados Studio. This member also determines the match score
        /// (in our implementation always 100%, as only exact matches are supported)
        /// as well as the confirmation lelvel, i.e. Translated.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="searchSegment"></param>
        /// <param name="searchResultSegment"></param>
        /// <param name="sourceSegment"></param>
        /// <param name="formattingPenalty"></param>
        /// <returns></returns>

        #region "CreateSearchResult"
        private SearchResult CreateSearchResult(SearchSettings settings, Segment searchSegment, Sdl.Community.Taus.TM.Provider.Segment.Segment searchResultSegment,
            string sourceSegment, bool formattingPenalty)
        {
            
            #region "TranslationUnit"
            var tu = new TranslationUnit();

           

            var searchSegmentSource = new Segment();
            var searchSegmentTarget = new Segment();

            

            searchSegmentSource.Add(searchResultSegment.SourceText);
            searchSegmentTarget.Add(searchResultSegment.TargetText);


            tu.SourceSegment = searchSegmentSource;
            tu.TargetSegment = searchSegmentTarget;

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            #endregion

       
            #region "TuProperties"
            var score = Convert.ToInt32(searchResultSegment.MatchPercentage);
            tu.Origin = TranslationUnitOrigin.TM;

            #region  |  properties  |
            if (searchResultSegment.Industry.Name.Trim() != string.Empty)
            {
                var fieldValue = new MultiplePicklistFieldValue("industry");
                fieldValue.Add(searchResultSegment.Industry.Name + " [" + searchResultSegment.Industry.Id + "]");
                tu.FieldValues.Add(fieldValue);
            }
            if (searchResultSegment.ContentType.Name.Trim() != string.Empty)
            {
                var fieldValue = new MultiplePicklistFieldValue("contentType");
                fieldValue.Add(searchResultSegment.ContentType.Name + " [" + searchResultSegment.ContentType.Id + "]");
                tu.FieldValues.Add(fieldValue);
            }
            if (searchResultSegment.Owner.Name.Trim() != string.Empty)
            {
                var fieldValue = new MultiplePicklistFieldValue("owner");
                fieldValue.Add(searchResultSegment.Owner.Name + " [" + searchResultSegment.Owner.Id + "]");
                tu.FieldValues.Add(fieldValue);
            }
            if (searchResultSegment.Product.Name.Trim() != string.Empty)
            {
                var fieldValue = new MultiplePicklistFieldValue("product");
                fieldValue.Add(searchResultSegment.Product.Name + " [" + searchResultSegment.Product.Id + "]");
                tu.FieldValues.Add(fieldValue);
            }
            if (searchResultSegment.Provider.Name.Trim() != string.Empty)
            {
                var fieldValue = new MultiplePicklistFieldValue("provider");
                fieldValue.Add(searchResultSegment.Provider.Name + " [" + searchResultSegment.Provider.Id + "]");
                tu.FieldValues.Add(fieldValue);
            }

            #endregion




            var searchResult = new SearchResult(tu) {ScoringResult = new ScoringResult {BaseScore = score}};


            if (settings.Mode == SearchMode.ConcordanceSearch)
            {
                searchSegmentSource.Tokens = Tokenize(searchSegmentSource);                
                searchResult.ScoringResult.MatchingConcordanceRanges = CollectConcordanceMatchRanges(searchSegmentSource, _visitor.PlainText);                
            }
            else if (settings.Mode == SearchMode.TargetConcordanceSearch)
            {
                searchSegmentTarget.Tokens = Tokenize(searchSegmentTarget);
                searchResult.ScoringResult.MatchingConcordanceRanges = CollectConcordanceMatchRanges(searchSegmentTarget, _visitor.PlainText); 
            }

            var providerPenalty = settings.FindPenalty(PenaltyType.ProviderPenalty);
            if (providerPenalty != null && providerPenalty.Malus > 0)
            {
                var penalty = new Penalty(PenaltyType.ProviderPenalty, settings.FindPenalty(PenaltyType.ProviderPenalty).Malus);
                searchResult.ScoringResult.ApplyPenalty(penalty);
            }

            if (searchResult.ScoringResult.BaseScore >= settings.MinScore)
            {
                searchResult.TranslationProposal = searchResult.MemoryTranslationUnit;

                if (searchSegment.HasTags)
                {
                    
                    #region "Draft"
                    tu.ConfirmationLevel = ConfirmationLevel.Draft;
                    #endregion

                    #region "FormattingPenalty"
                    var penalty = new Penalty(PenaltyType.MemoryTagsDeleted, settings.FindPenalty(PenaltyType.MemoryTagsDeleted).Malus);
                    searchResult.ScoringResult.ApplyPenalty(penalty);
                    #endregion
                }
                else
                {
                    
                    tu.ConfirmationLevel = ConfirmationLevel.Translated;
                }
              
            }
            else
            {
                searchResult = null;
            }
            #endregion

            return searchResult;
        }
        #endregion



        public bool CanReverseLanguageDirection
        {
            get { return false; }
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
            {
                throw new ArgumentNullException(@"segments in SearchSegmentsMasked");
            }
            if (mask == null || mask.Length != segments.Length)
            {
                throw new ArgumentException(@"mask in SearchSegmentsMasked");
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
           
            var s = new Segment(_languageDirection.SourceCulture);
            s.Add(segment);
            return SearchSegment(settings, s);
        }

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            var searchResult = new SearchResults();

            if (translationUnit.TargetSegment !=null)
            {
                if (_options.IgnoreTranslatedSegments == "False")
                {
                    searchResult = SearchSegment(settings, translationUnit.SourceSegment);
                }
                else
                {
                    if (translationUnit.TargetSegment.IsEmpty)
                        searchResult = SearchSegment(settings, translationUnit.SourceSegment);
                    else
                    {
                        searchResult.SourceSegment = translationUnit.SourceSegment.Duplicate();
                    }
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

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            var results = new List<SearchResults>();

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
            throw new NotImplementedException();
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
