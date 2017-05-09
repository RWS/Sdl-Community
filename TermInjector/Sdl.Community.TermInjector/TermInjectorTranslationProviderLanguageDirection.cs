using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TermInjector
{
    public class TermInjectorTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        private static LanguagePair _languageDirection;
        
        #region "PrivateMembers"
        private TermInjectorTranslationProvider _provider;
        private static TermInjectorTranslationOptions _options;
        
        private Boolean _matchCase;
        private SearchResults[] _resultarray;

        #endregion

        #region "ITranslationProviderLanguageDirection Members"

        #region "TermInjectorTranslationProviderLanguageDirection"
        public TermInjectorTranslationProviderLanguageDirection(TermInjectorTranslationProvider provider, LanguagePair languages)
        {
            #region "Instantiate"
            _provider = provider;
            _languageDirection = languages;
            _options = _provider.Options;
            _matchCase = _options.MatchCase == "true" ? true : false;
            #endregion

        }

        #endregion

        public System.Globalization.CultureInfo SourceLanguage
        {
            get { return _languageDirection.SourceCulture; }
        }

        public System.Globalization.CultureInfo TargetLanguage
        {
            get { return _languageDirection.TargetCulture; }
        }

        public bool CanReverseLanguageDirection
        {
            get { return false; }
        }

        public ITranslationProvider TranslationProvider
        {
            get { return _provider; }
        }

        private Boolean InjectTermsNoMatch(Segment segment, Segment blankSegment)
        {
            //Reset the nomatch visitor and have it visit the segment elements
            _provider.NoMatchVisitor.Reset();
            //Pass the blank segment to no match visitor
            _provider.NoMatchVisitor.Segment = blankSegment;


            foreach (var element in segment.Elements)
            {

                element.AcceptSegmentElementVisitor(_provider.NoMatchVisitor);
            }


            if (_provider.NoMatchVisitor.OriginalSegmentChanged)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Segment InjectTermsFuzzyMatch(Segment segment, SearchResult result)
        {
            
            //Reset the fuzzy current visitor
            _provider.FuzzyCurrentVisitor.Reset();

            //Have the fuzzy visitor go through the current source segment and pick up found
            //terms and their possible replacements
            foreach (var element in segment.Elements)
            {
                element.AcceptSegmentElementVisitor(_provider.FuzzyCurrentVisitor);
            }

            //Generate a new replacement trie from the fuzzy current visitor term list
            Trie replacementTrie = _provider.ExactMatchTrieReplaces.Clone();
            
            //Create a new regex trie which will be used as the secondary regex trie in the visitor
            RegexTrie<TranslationAndReplacement> replacementRegexTrie = new RegexTrie<TranslationAndReplacement>();
            Boolean regexesAdded = false;
            
            //This will be the new term list of the visitor, with replacement terms removed
            List<PositionAndTranslation> newTermList = new List<PositionAndTranslation>();

            //Go through the terms adding the terms to either the normal or regex trie or to the new term list
            foreach (var term in _provider.FuzzyCurrentVisitor.TermList)
            {
                //If the term has a replaces value, add it to trie
                if (term.Replaces != "")
                {
                    //Use replaces field as source. If the term is from a regex trie, add
                    //it to a regex trie
                    if (term.Regex)
                    {
                        _provider.RegexTrieFactory.AddToRegexTrie(
                            replacementRegexTrie,
                            term.Replaces,
                            new TranslationAndReplacement(
                                term.Translation,
                                ""));
                        regexesAdded = true;
                        
                    }
                    else
                    {
                        replacementTrie.AddToTrie(term.Replaces, term.Translation, "");
                    }
                }
                //If there's no replaces field, handle this in the comparison phase
                else
                {
                    newTermList.Add(term);
                }
            }
            //Determinise the regex trie
            if (regexesAdded)
            {
                replacementRegexTrie = _provider.Determiniser.determiniseNFA(replacementRegexTrie);
            }
            //Update the fuzzy term list with the list that does not contain the replacement terms
            _provider.FuzzyCurrentVisitor.TermList = newTermList;

            //Reset fuzzy replace visitor and update the tries to it
            _provider.FuzzyReplaceVisitor.Reset();
            _provider.FuzzyReplaceVisitor.SndTrie = replacementTrie;
            _provider.FuzzyReplaceVisitor.SndRegexTrie = replacementRegexTrie;
            
            //Go through the translation proposal target segment with the visitor
            foreach (var element in result.TranslationProposal.TargetSegment.Elements)
            {
                //Why would there be null elements? 
                if (element != null)
                {
                    element.AcceptSegmentElementVisitor(_provider.FuzzyReplaceVisitor);
                }
            }

            Segment segmentWithTerms = _provider.FuzzyReplaceVisitor.Segment;

            //Visit the elements of the results, compare the resulting visitor with the _fuzzyVisitor and
            //and add the terms only found in _fuzzyVisitor to the translation proposal
            if (_options.InjectNewTermsIntoFuzzies == "true")
            {
                _provider.FuzzyMatchVisitor.Reset();
                foreach (var element in result.MemoryTranslationUnit.SourceSegment.Elements)
                {
                    element.AcceptSegmentElementVisitor(_provider.FuzzyMatchVisitor);
                }

                Text newTerms = _provider.FuzzyCurrentVisitor.TermDifference(_provider.FuzzyMatchVisitor);

                if (newTerms.Value.Length > 0)
                {
                    segmentWithTerms.Elements.Insert(0, newTerms);
                }
            }

            //Return a deep copy of the segment (if reference is used, all results will display
            //the last segment constructed.)
            return segmentWithTerms.Duplicate();
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            //This is the function that is called when a TM lookup is done, so the injection code is called here
            #region "SegmentLookup"
            if (settings.Mode == SearchMode.NormalSearch || settings.Mode == SearchMode.FullSearch)
            {
                _resultarray = _provider.FileTM.GetLanguageDirection(_languageDirection).SearchTranslationUnitsMasked(settings,translationUnits,mask);
                
                foreach (var results in _resultarray)
                {
                    //First search the TM for results. If there are no results, give as a result the source segment
                    //with terms translations inserted
                    if (results == null)
                    {
                        continue;
                    }
                    if (results.Count == 0)
                    {
                        
                        //This segment is passed onto the injection method and possible modified
                        Segment blankSegment = new Segment();

                        //If the injection function changes the segment, add it as a result
                        if (InjectTermsNoMatch(results.SourceSegment, blankSegment))
                        {
                            results.Add(CreateSearchResult(results.SourceSegment, blankSegment));
                        }
                    }
                    //If there are fuzzy matches, check if there are replacements for the terms in the
                    //fuzzy segment, and add translations of new terms to the beginning of the segment
                    else
                    {
                        foreach (var result in results)
                        {
                            //If a 100 percent base score is found, check whether 100 percent matches
                            //should also be edited. If not, return the TM results as they are
                            //(this is the default, usually no need to mess with 100 percent matches)
                            if ((result.ScoringResult.BaseScore == 100) && _options.InjectIntoFullMatches == "false")
                            {
                                return _provider.FileTM.GetLanguageDirection(_languageDirection).SearchTranslationUnitsMasked(settings, translationUnits, mask);
                            }

                            Segment termSegment = new Segment();
                            
                            termSegment = InjectTermsFuzzyMatch(results.SourceSegment, result);
                            result.TranslationProposal.TargetSegment = termSegment;
                            result.MemoryTranslationUnit.TargetSegment = termSegment;
                        }
                        
                        
                    }

                    return _resultarray;
                }
            }
            #endregion

            // These search modes are simply wrappers for TM search modes
            #region "Wrapped search modes"
            if (settings.Mode == SearchMode.ConcordanceSearch ||
                settings.Mode == SearchMode.TargetConcordanceSearch ||
                settings.Mode == SearchMode.DuplicateSearch ||
                settings.Mode == SearchMode.ExactSearch)
            {
                _resultarray = _provider.FileTM.GetLanguageDirection(_languageDirection).SearchTranslationUnitsMasked(settings,translationUnits,mask);
            }
            #endregion
            return _resultarray;
        }

        #region "SearchSegment"
        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            return _provider.FileTM.GetLanguageDirection(_languageDirection).SearchSegment(settings, segment);
        }
        #endregion


        /// Creates the translation unit as it is later shown in the Translation Results
        /// window of SDL Trados Studio.

        #region "CreateSearchResult"
        private SearchResult CreateSearchResult(Segment searchSegment, Segment translation)
        {
            #region "TranslationUnit"
            TranslationUnit tu = new TranslationUnit();
            Segment orgSegment = searchSegment;
            tu.SourceSegment = orgSegment;
            tu.TargetSegment = translation;
            
            #endregion

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            #region "TuProperties"
            int score = _options.NewSegmentPercentage;
            tu.Origin = TranslationUnitOrigin.TM;


            SearchResult searchResult = new SearchResult(tu);
            searchResult.TranslationProposal = tu;
            searchResult.ScoringResult = new ScoringResult();
            searchResult.ScoringResult.BaseScore = score;
            

            tu.ConfirmationLevel = ConfirmationLevel.Unspecified;
            #endregion

            return searchResult;
        }
        #endregion

        //Concordance search uses this method. This plugin was developed before custom UI stuff existed in the API,
        //so this was used as a hacky way of adding terms in the Studio UI. So when you run concordance search on a
        //text that contains the term addition separator, the term is added to the rule file.
        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            _provider.addStringToTries(segment);
            return _provider.FileTM.GetLanguageDirection(_languageDirection).SearchText(settings, segment);
        }

        //TermInjector doesn't need to modify these methods, so they are implemented as wrappers for the corresponding TM methods
        #region "Wrappers for TM methods"

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            //MessageBox.Show("seseg");
            return _provider.FileTM.GetLanguageDirection(_languageDirection).SearchSegments(settings,segments);
        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            //MessageBox.Show("sesegmask");
            return _provider.FileTM.GetLanguageDirection(_languageDirection).SearchSegmentsMasked(settings, segments, mask);
        }

        

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            //MessageBox.Show("setu");
            return _provider.FileTM.GetLanguageDirection(_languageDirection).SearchTranslationUnit(settings, translationUnit);
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            //MessageBox.Show("setus");
            return _provider.FileTM.GetLanguageDirection(_languageDirection).SearchTranslationUnits(settings, translationUnits);
            
        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {

            return _provider.FileTM.GetLanguageDirection(_languageDirection).AddTranslationUnitsMasked(translationUnits, settings, mask);
            
        }

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            return _provider.FileTM.GetLanguageDirection(_languageDirection).UpdateTranslationUnit(translationUnit);
            
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            return _provider.FileTM.GetLanguageDirection(_languageDirection).UpdateTranslationUnits(translationUnits);
            
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            return _provider.FileTM.GetLanguageDirection(_languageDirection).AddOrUpdateTranslationUnitsMasked(translationUnits,previousTranslationHashes,settings,mask);
            
        }

        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            return _provider.FileTM.GetLanguageDirection(_languageDirection).AddTranslationUnit(translationUnit,settings);
            
        }

        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            return _provider.FileTM.GetLanguageDirection(_languageDirection).AddTranslationUnits(translationUnits,settings);
            
        }

        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            return _provider.FileTM.GetLanguageDirection(_languageDirection).AddOrUpdateTranslationUnits(translationUnits,previousTranslationHashes,settings);
            
        }
        #endregion

        #endregion
    }
}
