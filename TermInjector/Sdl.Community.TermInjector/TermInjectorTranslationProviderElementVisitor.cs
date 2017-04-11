using System;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.TermInjector
{
    
    //This is the visitor used for no match segments, it will construct a segment
    //where terms found in the source are replaced with their translations
    //This visitor is also used for replacing terms in fuzzy segments. Hence the
    //sndRegexTrie, which is used to keep the fuzzy replacement regexes which are
    //conditional on some word appearing in the source (which means they cannot
    //be included in the general Regex Trie)
    public class TermInjectorTranslationProviderElementTermReplacementVisitor : ISegmentElementVisitor
    {
        private Trie _trie;
        private Trie _sndTrie;
        private RegexTrie<TranslationAndReplacement> _regexTrie;
        private RegexTrie<TranslationAndReplacement> _sndRegexTrie;
        private TrieProcessing _trieProcessor;
        private TermInjectorTranslationOptions _options;
        private Segment _segment;
        private List<PositionAndTranslation> _positionAndTranslationOfTerms;
        private Boolean _originalSegmentChanged;

        public Segment Segment
        {
            get
            {
                if (_segment == null)
                {
                    _segment = new Segment();
                }
                return _segment;
            }
            set
            {
                _segment = value;
            }
        }

        public Trie Trie
        {
            get
            {
                if (_trie == null)
                {
                    _trie = new Trie();
                }
                return _trie;
            }
            set
            {
                _trie = value;
            }
        }

        public Trie SndTrie
        {
            get
            {
                if (_sndTrie == null)
                {
                    _sndTrie = new Trie();
                }
                return _trie;
            }
            set
            {
                _sndTrie = value;
            }
        }

        public RegexTrie<TranslationAndReplacement> RegexTrie
        {
            get
            {
                if (_regexTrie == null)
                {
                    _regexTrie = new RegexTrie<TranslationAndReplacement>();
                }
                return _regexTrie;
            }
            set
            {
                _regexTrie = value;
            }
        }

        public RegexTrie<TranslationAndReplacement> SndRegexTrie
        {
            get
            {
                if (_sndRegexTrie == null)
                {
                    _sndRegexTrie = new RegexTrie<TranslationAndReplacement>();
                }
                return _sndRegexTrie;
            }
            set
            {
                _sndRegexTrie = value;
            }
        }

        public Boolean OriginalSegmentChanged
        {
            get
            {
                return _originalSegmentChanged;
            }
            set
            {
                _originalSegmentChanged = value;
            }
        }

        public void Reset()
        {
            _segment.Clear();
            _originalSegmentChanged = false;
        }

        public TermInjectorTranslationProviderElementTermReplacementVisitor(
            TermInjectorTranslationOptions options)
        {
            _options = options;
            _segment = new Segment();
            //Create a new trie processor
            _trieProcessor = new TrieProcessing();
            //Initialize the dictionary which will contain the positions and translations
            //of terms
            _positionAndTranslationOfTerms = new List<PositionAndTranslation>();
        }

        public TermInjectorTranslationProviderElementTermReplacementVisitor(
            TermInjectorTranslationOptions options,
            Trie glossaryTrie,
            RegexTrie<TranslationAndReplacement> regexTrie)
        {
            _options = options;
            _segment = new Segment();
            //Initialize the glossary trie
            _trie = glossaryTrie;
            //Initialize the regex trie
            _regexTrie = regexTrie;
            //Create a new trie processor
            _trieProcessor = new TrieProcessing();
            //Initialize the dictionary which will contain the positions and translations
            //of terms
            _positionAndTranslationOfTerms = new List<PositionAndTranslation>();
            //Boolean for indicating whether the original segment has been changed
            _originalSegmentChanged = false;
        }

        #region ISegmentElementVisitor Members

        //All visit methods except VisitText simply copy source tokens
        public void VisitDateTimeToken(Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken token)
        {
            _segment.Add(token);
        }

        public void VisitMeasureToken(Sdl.LanguagePlatform.Core.Tokenization.MeasureToken token)
        {
            _segment.Add(token);
        }

        public void VisitNumberToken(Sdl.LanguagePlatform.Core.Tokenization.NumberToken token)
        {
            _segment.Add(token);
        }

        public void VisitSimpleToken(Sdl.LanguagePlatform.Core.Tokenization.SimpleToken token)
        {
            _segment.Add(token);
        }

        public void VisitTag(Tag tag)
        {
            _segment.Add(tag);
        }

        public void VisitTagToken(Sdl.LanguagePlatform.Core.Tokenization.TagToken token)
        {
            _segment.Add(token);
        }


        /// <summary>
        /// First the match case option is checked, then the term translations and their
        /// source term positions are retrieved with FindMatches. Finally the term
        /// translations are inserted with InjectMatches
        /// </summary>
        /// <param name="text"></param>
        public void VisitText(Text text)
        {
            bool matchCase = _options.MatchCase == "true" ? true : false;
            bool useBoundaryChars = _options.UseBoundaryCharacters == "true" ? true : false;

            //List containing all four types of matches: normal and regex matches plus normal and regex replace matches
            List<PositionAndTranslation> allMatches = new List<PositionAndTranslation>();

            //Add normal and regex matches

            allMatches.AddRange(_trieProcessor.FindMatches(this._trie, text.ToString(), _options.TokenBoundaryCharacters, matchCase, useBoundaryChars));
            allMatches.AddRange(_trieProcessor.FindRegexMatches(this._regexTrie,
                text.ToString(),
                _options.TokenBoundaryCharacters,
                useBoundaryChars));

            //Add the results of the secondary regex tries to the match list
            //The match discardal in the case of secondary regex trie matches using groups should be implemented here, possibly with a switch
            allMatches.AddRange(_trieProcessor.FindRegexMatches(
                this.SndRegexTrie, text.ToString(), _options.TokenBoundaryCharacters,useBoundaryChars));
            allMatches.AddRange(_trieProcessor.FindMatches(
                this.SndTrie, text.ToString(), _options.TokenBoundaryCharacters, matchCase, useBoundaryChars));
            
            //If there are matches, remove the overlaps and inject them
            if (allMatches.Count > 0)
            {
                this._positionAndTranslationOfTerms = _trieProcessor.RemoveOverLaps(allMatches);
                this._segment.Add(_trieProcessor.InjectMatches(text.ToString(), _positionAndTranslationOfTerms));
                this._originalSegmentChanged = true;
            }
            else
            {
                _segment.Add(text);
            }
        }

        #endregion
    }

    public class TermInjectorTranslationProviderElementTermExtractionVisitor : ISegmentElementVisitor
    {

        //This is the visitor used for fuzzy segments, it will collect a list of terms
        //present in the segment. Such lists are collected for both current segments source and
        //the translation proposal's source. These lists are then compared and the translations
        //of terms not present in the translation proposal's source are inserted at the start
        //of the translation proposal's target segment.

        private Trie _trie;
        private RegexTrie<TranslationAndReplacement> _regexTrie;
        private TrieProcessing _trieProcessor;
        private TermInjectorTranslationOptions _options;
        private List<PositionAndTranslation> _TermList;

        public List<PositionAndTranslation> TermList
        {
            get
            {
                if (_TermList == null)
                {
                    _TermList = new List<PositionAndTranslation>();
                }
                return _TermList;
            }
            set
            {
                _TermList = value;
            }
        }

        public Trie Trie
        {
            get
            {
                if (_trie == null)
                {
                    _trie = new Trie();
                }
                return _trie;
            }
            set
            {
                _trie = value;

            }
        }

        public RegexTrie<TranslationAndReplacement> RegexTrie
        {
            get
            {
                if (_regexTrie == null)
                {
                    _regexTrie = new RegexTrie<TranslationAndReplacement>();
                }
                return _regexTrie;
            }
            set
            {
                _regexTrie = value;
            }
        }
        
        public void Reset()
        {
            _TermList.Clear();
        }

        public TermInjectorTranslationProviderElementTermExtractionVisitor(
            TermInjectorTranslationOptions options,
            Trie glossaryTrie,
            RegexTrie<TranslationAndReplacement> regexTrie)
        {
            _options = options;
            _trieProcessor = new TrieProcessing();
            _trie = glossaryTrie;
            
            //Initialize the regex trie
            _regexTrie = regexTrie;
            
            _TermList = new List<PositionAndTranslation>();
        }

        //This compares the visitor to another fuzzy visitor and return a Text element containing all terms only
        //contained in this visitor
        public Text TermDifference(TermInjectorTranslationProviderElementTermExtractionVisitor comparisonVisitor)
        {
            Text termElement = new Text();
            StringBuilder termString = new StringBuilder();
            Boolean isNewTerm = true;
            foreach (var term in this.TermList)
            {
                //If there's a replace field, this has already been handled in the replace visitor
                if (term.Replaces != "")
                {
                    continue;
                }
                isNewTerm = true;
                foreach (var comparisonTerm in comparisonVisitor.TermList)
                {
                    if (term.Translation.Equals(comparisonTerm.Translation))
                    {
                        isNewTerm = false;
                        break;
                    }
                }
                if (isNewTerm)
                {
                    termString.Append(term.Translation + " ");
                }
            }
            termElement.Value = termString.ToString();
            return termElement;
        }

        #region ISegmentElementVisitor Members

        //All visit methods except VisitText simply do nothing

        public void VisitDateTimeToken(Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken token)
        {
        }

        public void VisitMeasureToken(Sdl.LanguagePlatform.Core.Tokenization.MeasureToken token)
        {
        }

        public void VisitNumberToken(Sdl.LanguagePlatform.Core.Tokenization.NumberToken token)
        {
        }

        public void VisitSimpleToken(Sdl.LanguagePlatform.Core.Tokenization.SimpleToken token)
        {
        }

        public void VisitTag(Tag tag)
        {
        }

        public void VisitTagToken(Sdl.LanguagePlatform.Core.Tokenization.TagToken token)
        {
        }

        /// <summary>
        /// First the match case option is checked, then the term translations and their
        /// source term positions are retrieved with FindMatches.
        /// </summary>
        /// <param name="text"></param>


        //This used to be broken (only the last element would be taken into account), I fixed it 15.11.2011
        public void VisitText(Text text)
        {
            bool matchCase = _options.MatchCase == "true" ? true : false;
            bool useBoundaryChars = _options.UseBoundaryCharacters == "true" ? true : false;
            List<PositionAndTranslation> allMatches = new List<PositionAndTranslation>();
            allMatches.AddRange(_trieProcessor.FindMatches(_trie, text.ToString(), _options.TokenBoundaryCharacters, matchCase,useBoundaryChars));
            allMatches.AddRange(_trieProcessor.FindRegexMatches(_regexTrie, text.ToString(), _options.TokenBoundaryCharacters,useBoundaryChars));
            _TermList.AddRange(_trieProcessor.RemoveOverLaps(allMatches));
        }

        #endregion
    }
}