using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Processors
{
    static class RegexProcessorHelper
    {
        internal static List<RegexMatch> ApplyRegexRules(string plainString, List<MatchRule> matchRules)
        {
            List<RegexMatch> searchResults = new List<RegexMatch>();

            //iterate over tag pairs first
            ProcessTagPairRules(plainString, matchRules, searchResults);
            //then iterate over all rules and treat them all as placeholders (doing actual placeholders first)
            ProcessPlaceholderRules(plainString, matchRules, searchResults);

            //sort results by match index so that they are in the correct order for later processing
            searchResults.Sort(new RegexSearchResultsComparer());

            return searchResults;
        }

        private static void ProcessTagPairRules(
            string plainString,
            IEnumerable<MatchRule> matchRules,
            ICollection<RegexMatch> searchResults)
        {
            var tagPairIntervals = new List<Tuple<RegexMatch, RegexMatch>>();

            foreach (var rule in matchRules)
            {
                if (rule.TagType == MatchRule.TagTypeOption.TagPair)
                {
                    Regex startTagRegex = rule.BuildStartTagRegex();
                    Regex endTagRegex = rule.BuildEndTagRegex();

                    //make sure that \n etc are ignored in the search
                    RegexOptions options = RegexOptions.Singleline | startTagRegex.Options;

                    //combine open and end tag regex and match all in between
                    Regex completeRegex = new Regex(startTagRegex +
                                                    @".*?" + endTagRegex, options);

                    Match completeMatch = completeRegex.Match(plainString);

                    //matches full tag pair, otherwise we could have orphaned start or end tags
                    while (completeMatch.Success)
                    {
                        Match startTagMatch = startTagRegex.Match(completeMatch.Value);
                        Debug.Assert(startTagMatch.Success, "Unable to find start tag match in tag pair search!");

                        Match endTagMatch = endTagRegex.Match(completeMatch.Value);
                        Debug.Assert(endTagMatch.Success, "Unable to find end tag match in tag pair search!");

                        int endIndex = (completeMatch.Index + completeMatch.Length) - endTagMatch.Value.Length;

                        var startMatch = CreateRegexMatch(rule, startTagMatch, completeMatch.Index,
                            RegexMatch.TagType.TagPairOpening);
                        var endMatch = CreateRegexMatch(rule, endTagMatch, endIndex, RegexMatch.TagType.TagPairClosing);

                        //only create tag pairs where we can, considering the existing ones
                        if (CheckIfTagPairCanBeCreatedForRange(startMatch, endMatch, tagPairIntervals))
                        {
                            searchResults.Add(startMatch);
                            searchResults.Add(endMatch);
                            tagPairIntervals.Add(new Tuple<RegexMatch, RegexMatch>(startMatch, endMatch));
                        }

                        completeMatch = completeMatch.NextMatch();
                    }
                }
            }
        }

        private static void ProcessPlaceholderRules(
            string plainString,
            List<MatchRule> matchRules,
            List<RegexMatch> searchResults)
        {
            //we need to process placeholders before orphaned tag pair start/end tags
            matchRules = MovePlaceholdersBeforeTagPairs(matchRules);

            foreach (MatchRule rule in matchRules)
            {
                Regex startTagRegex = rule.BuildStartTagRegex();
                Match startTagMatch = startTagRegex.Match(plainString);

                while (startTagMatch.Success)
                {
                    if (CheckIfPlaceholderCanBeCreatedForMatch(startTagMatch, searchResults))
                    {
                        searchResults.Add(CreateRegexMatch(rule, startTagMatch, startTagMatch.Index,
                            RegexMatch.TagType.Placeholder));
                    }

                    startTagMatch = startTagMatch.NextMatch();
                }

                //process tag pair end tags
                if (rule.TagType == MatchRule.TagTypeOption.TagPair)
                {
                    Regex endTagRegex = rule.BuildEndTagRegex();
                    Match endTagMatch = endTagRegex.Match(plainString);

                    while (endTagMatch.Success)
                    {
                        if (CheckIfPlaceholderCanBeCreatedForMatch(endTagMatch, searchResults))
                        {
                            searchResults.Add(CreateRegexMatch(rule, endTagMatch, endTagMatch.Index,
                                RegexMatch.TagType.Placeholder));
                        }

                        endTagMatch = endTagMatch.NextMatch();
                    }
                }
            }
        }

        private static List<MatchRule> MovePlaceholdersBeforeTagPairs(List<MatchRule> matchRules)
        {
            var result = new List<MatchRule>();

            matchRules.ForEach(matchRule =>
            {
                if (matchRule?.TagType == MatchRule.TagTypeOption.Placeholder)
                    result.Add(matchRule);
            });

            matchRules.ForEach(matchRule =>
            {
                if (matchRule?.TagType != MatchRule.TagTypeOption.Placeholder)
                    result.Add(matchRule);
            });

            return result;
        }

        private static bool CheckIfPlaceholderCanBeCreatedForMatch(Match match, IEnumerable<RegexMatch> searchResults)
        {
            return searchResults.All(s =>
                match.Index >= s.Index + s.Value.Length
                || match.Index + match.Value.Length <= s.Index);
        }

        private static bool CheckIfTagPairCanBeCreatedForRange(
            RegexMatch startMatch,
            RegexMatch endMatch,
            List<Tuple<RegexMatch, RegexMatch>> tagPairIntervals)
        {
            var newInterval = new Tuple<RegexMatch, RegexMatch>(startMatch, endMatch);

            return tagPairIntervals.All(x =>
                IsFirstContainedInSecond(newInterval, x) ||
                IsFirstContainedInSecond(x, newInterval) ||
                DoesFirstEndBeforeSecond(newInterval, x) ||
                DoesFirstEndBeforeSecond(x, newInterval));
        }

        private static bool IsFirstContainedInSecond(
            Tuple<RegexMatch, RegexMatch> first,
            Tuple<RegexMatch, RegexMatch> second)
        {
            return first.Item1.Index >= second.Item1.Index + second.Item1.Value.Length
                   && first.Item2.Index + first.Item2.Value.Length <= second.Item2.Index;
        }

        private static bool DoesFirstEndBeforeSecond(
            Tuple<RegexMatch, RegexMatch> first,
            Tuple<RegexMatch, RegexMatch> second)
        {
            return first.Item2.Index + first.Item2.Value.Length <= second.Item1.Index;
        }

        internal static bool HasEmbeddedContext(IList<string> structureInfos, IContextProperties contexts)
        {
            if (contexts == null)
            {
                return false;
            }

            foreach (var context in contexts.Contexts)
            {
                if (structureInfos.Contains(context.ContextType))
                {
                    return true;
                }
            }

            return false;
        }

        internal static IStartTagProperties CreateStartTagProperties(IPropertiesFactory factory, string tagContent,
            MatchRule rule)
        {
            IStartTagProperties startProperties = factory.CreateStartTagProperties(tagContent);

            ApplyInlineTagProperties(startProperties, rule);
            startProperties.DisplayText = GetDisplayName(tagContent);
            startProperties.Formatting = FormattingInflator.InflateFormatting(rule.Formatting);
            startProperties.SegmentationHint = rule.SegmentationHint;
            //startProperties.StoreCustomProperty("PairID", rule.GetHashCode().ToString());
            return startProperties;
        }

        internal static IEndTagProperties CreateEndTagProperties(IPropertiesFactory factory, string tagContent,
            MatchRule rule)
        {
            IEndTagProperties endProperties = factory.CreateEndTagProperties(tagContent);

            ApplyInlineTagProperties(endProperties, rule);
            endProperties.DisplayText = GetDisplayName(tagContent);
            // endProperties.StoreCustomProperty("PairID", rule.GetHashCode().ToString());

            return endProperties;
        }

        internal static IPlaceholderTagProperties CreatePlaceholderTagProperties(IPropertiesFactory factory,
            string tagContent, MatchRule rule)
        {
            IPlaceholderTagProperties placeholderProps = factory.CreatePlaceholderTagProperties(tagContent);

            ApplyInlineTagProperties(placeholderProps, rule);
            placeholderProps.DisplayText = GetDisplayName(tagContent);
            placeholderProps.SegmentationHint = rule.SegmentationHint;
            placeholderProps.TagContent = tagContent;

            if (!String.IsNullOrEmpty(rule.TextEquivalent))
            {
                placeholderProps.TextEquivalent = rule.TextEquivalent;
            }

            return placeholderProps;
        }

        internal static string GetDisplayName(string tagContent)
        {
            //trim the start and end to get rid of opening and closing XML tags
            string displayName = tagContent.TrimStart('<', '/');
            displayName = displayName.TrimEnd('>', '/');

            int spaceIndex = displayName.IndexOf(" ");
            if (spaceIndex > 0 && spaceIndex < 10)
            {
                //return first word
                displayName = displayName.Substring(0, spaceIndex);
            }
            else if (displayName.Length > 10)
            {
                //restrict to first 10 characters
                displayName = displayName.Substring(0, 10);
            }

            return displayName;
        }

        private static void ApplyInlineTagProperties(IAbstractInlineTagProperties tagProperties, MatchRule rule)
        {
            tagProperties.CanHide = rule.CanHide;
            tagProperties.IsSoftBreak = rule.IsSoftBreak;
            tagProperties.IsWordStop = rule.IsWordStop;
        }

        private static RegexMatch CreateRegexMatch(MatchRule rule, Match tagMatch, int index,
            RegexMatch.TagType tagType)
        {
            RegexMatch regexMatch = new RegexMatch();
            regexMatch.Type = tagType;
            regexMatch.Value = tagMatch.Value;
            regexMatch.Index = index;
            regexMatch.Rule = rule;
            return regexMatch;
        }
    }

    class RegexSearchResultsComparer : IComparer<RegexMatch>
    {
        #region IComparer<RegExSearchResult> Members

        public int Compare(RegexMatch x, RegexMatch y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.Index.CompareTo(y.Index);
        }

        #endregion
    }
}
