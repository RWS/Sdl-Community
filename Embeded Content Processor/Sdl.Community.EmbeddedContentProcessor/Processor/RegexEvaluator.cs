using System;
using System.Data;
using System.Text.RegularExpressions;
using Sdl.Community.EmbeddedContentProcessor.Infrastructure;
using Sdl.Community.EmbeddedContentProcessor.Settings;
using System.Collections.Generic;


namespace Sdl.Community.EmbeddedContentProcessor.Processor
{
    public class RegexEvaluator : IContentEvaluator
    {
        public List<ContentMatch> Evaluate(string text, List<MatchRule> rules)
        {
            var matchedIndexes = new List<int>();

            //iterate over tag pairs first
           var searchMatches = ProcessTagPairRules(matchedIndexes,rules,text);
            //the iterate over all rules and treat them as placeholders
           searchMatches.AddRange(ProcessPlaceholderRules(text,matchedIndexes, rules));


           //sort results by match index so that they are in the correct order for later processing
           searchMatches.Sort(new RegexSearchResultsComparer());
            return searchMatches;
        }

        private IEnumerable<ContentMatch> ProcessPlaceholderRules(string text, ICollection<int> matchedIndexes, List<MatchRule> rules)
        {
            var searchMatches = new List<ContentMatch>();

            //we need to process placeholders before tag pairs
            rules.Sort(new MatchRulesTypeComparer());

            foreach (var rule in rules)
            {
                var startTagRegex = rule.BuildStartTagRegex();
                var placeholderMatch = startTagRegex.Match(text);

                while (placeholderMatch.Success)
                {
                    if (!matchedIndexes.Contains(placeholderMatch.Index))
                    {
                        searchMatches.Add(CreateRegexMatch(rule, placeholderMatch, placeholderMatch.Index, ContentMatch.TagType.Placeholder));
                        matchedIndexes.Add(placeholderMatch.Index);
                    }

                    placeholderMatch = placeholderMatch.NextMatch();
                }

                //process tag pair end tags
                if (rule.TagType != MatchRule.TagTypeOption.TagPair) continue;
                var endTagRegex = rule.BuildEndTagRegex();
                var endTagMatch = endTagRegex.Match(text);

                while (endTagMatch.Success)
                {
                    if (!matchedIndexes.Contains(endTagMatch.Index))
                    {
                        searchMatches.Add(CreateRegexMatch(rule, endTagMatch, endTagMatch.Index, ContentMatch.TagType.Placeholder));
                        matchedIndexes.Add(endTagMatch.Index);
                    }

                    endTagMatch = endTagMatch.NextMatch();
                }
            }

            return searchMatches;
        }

        private ContentMatch CreateRegexMatch(MatchRule rule, Match startTagMatch, int index, ContentMatch.TagType tagType)
        {
            var regexMatch = new ContentMatch {Type = tagType, Value = startTagMatch.Value, Index = index, MatchRule = rule};
            return regexMatch;
        }

        private List<ContentMatch> ProcessTagPairRules(ICollection<int> matchedIndexes, IEnumerable<MatchRule> rules,string text)
        {
            if (matchedIndexes == null) throw new ArgumentNullException("matchedIndexes");
            var searchMatches = new List<ContentMatch>();

            foreach (var rule in rules)
            {
                if (rule.TagType != MatchRule.TagTypeOption.TagPair) continue;
                var startTagRegex = rule.BuildStartTagRegex();
                var endTagRegex = rule.BuildEndTagRegex();

                //make sure that \n etc are ignored in the search
                var options = RegexOptions.Singleline | startTagRegex.Options;

                //combine open and end tag regex and match all in between
                var completeRegex = new Regex(startTagRegex +
                                              @".*?" + endTagRegex, options);

                Match completeMatch = completeRegex.Match(text);

                //matches full tag pair, otherwise we could have orphaned start or end tags
                while (completeMatch.Success)
                {
                    Match startTagMatch = startTagRegex.Match(completeMatch.Value);

                    Match endTagMatch = endTagRegex.Match(completeMatch.Value);

                    int endIndex = (completeMatch.Index + completeMatch.Length) - endTagMatch.Value.Length;

                    //only add matches at indexes that have not already been added
                    if (!matchedIndexes.Contains(completeMatch.Index) && !matchedIndexes.Contains(endIndex))
                    {
                        //have to use index from original match when adding to search results or won't be valid in the document
                        searchMatches.Add(CreateRegexMatch(rule, startTagMatch, completeMatch.Index, ContentMatch.TagType.TagPairOpening));
                        searchMatches.Add(CreateRegexMatch(rule, endTagMatch, endIndex, ContentMatch.TagType.TagPairClosing));

                        matchedIndexes.Add(completeMatch.Index);
                        matchedIndexes.Add(endIndex);
                    }

                    completeMatch = completeMatch.NextMatch();
                }
            }

            return searchMatches;
        }
       
    }

    class RegexSearchResultsComparer : IComparer<ContentMatch>
    {
        #region IComparer<RegExSearchResult> Members

        public int Compare(ContentMatch x, ContentMatch y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.Index.CompareTo(y.Index);
        }

        #endregion
    }

    class MatchRulesTypeComparer : IComparer<MatchRule>
    {
        #region IComparer<MatchRule> Members

        public int Compare(MatchRule x, MatchRule y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            if (x.TagType == MatchRule.TagTypeOption.TagPair && y.TagType == MatchRule.TagTypeOption.Placeholder)
            {
                return 1;
            }
            if (x.TagType == MatchRule.TagTypeOption.Placeholder && y.TagType == MatchRule.TagTypeOption.TagPair)
            {
                return -1;
            }

            return 0;
        }

        #endregion
    }
}
