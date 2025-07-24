using QATracker.Components.SettingsProvider.Verifiers.BaseClass;
using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;
using QATracker.Components.SettingsProvider.Model;

namespace QATracker.Components.SettingsProvider.Verifiers
{
    public class QaVerificationSettings : VerificationSettings, IVerificationSettings
    {
        public QaVerificationSettings()
        {
            Name = Constants.QaCheckerVerifierName;
            Values =
            [
                new()
                {
                    Name = Constants.SegmentsVerification,
                    Values =
                    [
                        new() { Name = "CheckUntranslatedSegments", Value = "True" },
                        new() { Name = "CheckIdenticalSegments", Value = "False" },
                        new() { Name = "IdenticalSegmentsIgnoreTags", Value = "False" },
                        new() { Name = "IdenticalSegmentsIgnoreCase", Value = "False" },
                        new()
                        {
                            Name = "CheckTargetShorter", Enabled = "False",
                            Values =
                            [
                                new() { Name = "ShorterByValue", Value = "50" }
                            ]
                        },
                        new()
                        {
                            Name = "CheckTargetLonger", Enabled = "False",
                            Values =
                            [
                                new() { Name = "LongerByValue", Value = "50" }
                            ]
                        },
                        new()
                        {
                            Name = "MinimalWordCountValue", Value = "2",
                            Values =
                            [
                                new(){Name = "DiffUnitWord", Value = "True"}
                            ]
                        },
                        new()
                        {
                            Name = "CheckForbiddenChar", Enabled = "False",
                            Values =
                            [
                                new() { Name = "ForbiddenCharsValue", Value = "" }
                            ]
                        },
                    ],
                },
                new()
                {
                    Name = Constants.SegmentsToExclude,
                    Values =
                    [
                        new() { Name = "ExcludePerfectMatchSegments", Value = "True" },
                        new() { Name = "ExcludeExactMatches", Value = "False" },
                        new() { Name = "ExcludeFuzzyMatches", Value = "False" },
                        new() { Name = "ExcludeFuzzyMatchValue", Value = "99" },
                        new() { Name = "ElementContextExclusionValue", Value = "True" },
                        new() { Name = "ExcludeNewTrans", Value = "False" },
                        new() { Name = "ExcludeRepetition", Value = "False" },
                        new() { Name = "ExcludeConfirmed", Value = "False" },
                        new() { Name = "ExcludeLocked", Value = "True" },
                        new() { Name = "ExcludeIdentical", Value = "False" },
                        new() { Name = "ElementContextExclusion", Value = "False" },
                        new() { Name = "ExclusionStringValue", Value = "True" },
                        new() { Name = "ExclusionStringUntranslated", Value = "False" },
                        new()
                        {
                            Name = "ExclusionStringIdentical",
                            Value = "False"
                        },
                        new() { Name = "ExclusionStringIgnoreCase", Value = "False" },
                        new() { Name = "ExclusionStringRegEx", Value = "False" },
                        new() { Name = "ReportAllNonExcluded", Value = "False" }
                    ]
                },
                new()
                {
                    Name = Constants.Inconsistencies,
                    Values =
                    [
                        new() { Name = "CheckInconsistencies", Value = "False" },
                        new() { Name = "InconsistenciesIgnoreTags", Value = "False" },
                        new() { Name = "InconsistenciesIgnoreCase", Value = "False" },
                        new() { Name = "CheckRepeatedWords", Value = "False" },
                        new() { Name = "RepeatedWordsNumbers", Value = "False" },
                        new() { Name = "RepeatedWordsIgnoreCase", Value = "False" },
                        new() { Name = "UneditedSegments", Value = "False" },
                        new() { Name = "UneditedSegmentsSelectedFuzzyOnly", Value = "True" },
                        new() { Name = "UneditedNotConfirmed", Value = "False" },
                        new() { Name = "OnlyCheckIfMatchScoresBelow", Value = "False" }
                    ],
                },
                new()
                {
                    Name = Constants.Punctuation,
                    Values =
                    [
                        new() { Name = "CheckPunctuationDifferences", Value = "False" },
                        new() { Name = "CheckSpanishPunctuation", Value = "False" },
                        new() { Name = "CheckPunctuationSpace", Value = "False" },
                        new() { Name = "PunctuationSpacesFrench", Value = "False" },
                        new() { Name = "CheckMultipleSpaces", Value = "False" },
                        new() { Name = "CheckMultipleDots", Value = "False" },
                        new() { Name = "Ignore3Dots", Value = "False" },
                        new() { Name = "ExtraEndSpace", Value = "False" },
                        new() { Name = "CheckInitialCaps", Value = "False" },
                        new() { Name = "CheckGlobalCaps", Value = "False" },
                        new() { Name = "CheckBrackets", Value = "False" }
                    ],
                },
                new()
                {
                    Name = Constants.Numbers,
                    Values =
                    [
                        new() { Name = "CheckNumbers", Value = "False" },
                        new() { Name = "CheckDates", Value = "False" },
                        new() { Name = "CheckTimes", Value = "False" },
                        new() { Name = "CheckMeasurements", Value = "False" }
                    ],
                },
                new()
                {
                    Name = Constants.WordList,
                    Values =
                    [
                        new() { Name = "CheckWordList", Value = "False" },
                        new() { Name = "WordListIgnoreCase", Value = "False" },
                        new() { Name = "WordListWholeWord", Value = "False" }
                    ],
                },
                new()
                {
                    Name = Constants.RegularExpressions,
                    Values =
                    [
                        new() { Name = "CheckRegEx", Value = "False" },
                        new()
                        {
                            Name = "WordListIgnoreCase", Value = "False"
                        } // Use a specific RegEx ignore case if available
                    ],
                },
                new()
                {
                    Name = Constants.TrademarkCheck,
                    Values =
                    [
                        new() { Name = "CheckTrademarks", Value = "False" }
                    ],
                },
                new()
                {
                    Name = Constants.LengthVerification,
                    Values =
                    [
                        new() { Name = "CheckAbsoluteLength", Value = "True" },
                        new() { Name = "CheckFilterLengthLimit", Value = "True" },
                        new() { Name = "AbsoluteCharCountValue", Value = "50" },
                        new() { Name = "CheckAllContexts", Value = "True" },
                        new() { Name = "AbsoluteLengthElements", Value = "" }
                    ],
                }
            ];
        }

        private void MakeSpecificChanges()
        {
            var diffUnitWord = this["DiffUnitWord"];
            var wordsOrChars = diffUnitWord.Value == "True" ? "words" : "characters";
            diffUnitWord.Value = wordsOrChars;
        }

        public override VerificationSettingsTreeNode ToSettingsValue()
        {
            MakeSpecificChanges();
            return base.ToSettingsValue();
        }

        public override Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
        {
            // Segments Verification
            ["CheckIdenticalSegments"] = "Source and target are identical",
            ["IdenticalSegmentsIgnoreTags"] = "Ignore tags",
            ["IdenticalSegmentsIgnoreCase"] = "Ignore case",
            ["CheckTargetShorter"] = "Shorter by (%)",
            ["CheckTargetLonger"] = "Longer by (%)",
            ["DiffUnitWord"] = "Based on",
            ["MinimalWordCountValue"] = "Ignore segments with fewer than # words",
            ["CheckUntranslatedSegments"] = "Check for forgotten and empty translations",
            ["CheckForbiddenChar"] = "Check for forbidden characters",
            ["ShorterByValue"] = "Shorter by value",
            ["LongerByValue"] = "Longer by value",
            ["ForbiddenCharsValue"] = "Forbidden chars",


            // Segment exclusion
            ["ExcludePerfectMatchSegments"] = "Exclude PerfectMatch and Context Match units",
            ["ExcludeLocked"] = "Exclude locked segments",

            ["ExcludeExactMatches"] = "Exclude exact matches",
            ["ExcludeFuzzyMatches"] = "Exclude fuzzy matches down to",
            ["ExcludeFuzzyMatchValue"] = "Exclude fuzzy match value",
            ["ElementContextExclusionValue"] = "Element context exclusion value",
            ["ExcludeNewTrans"] = "Exclude new translations",
            ["ExcludeRepetition"] = "Exclude repetitions",
            ["ExcludeConfirmed"] = "Exclude confirmed translations",
            ["ExcludeIdentical"] = "Exclude target segment identical to source",
            ["ExclusionStringValue"] = "Exclusion string value",
            ["ElementContextExclusion"] = "Check only segments which are in following contexts",
            ["ExclusionStringUntranslated"] = "Apply exclusion list to search for not translated segments",
            ["ExclusionStringIdentical"] = "Apply exclusion list to search for target segments identical to source",
            ["ExclusionStringIgnoreCase"] = "Ignore case",
            ["ExclusionStringRegEx"] = "Exclusions use regular expressions",
            ["ReportAllNonExcluded"] = "Report all non-excluded segments",

            // Inconsistencies
            ["CheckInconsistencies"] = "Check for inconsistent translations",
            ["InconsistenciesIgnoreCase"] = "Ignore case",
            ["InconsistenciesIgnoreTags"] = "Ignore tags",
            ["CheckRepeatedWords"] = "Check for repeated words in target",
            ["RepeatedWordsNumbers"] = "Ignore numbers",
            ["RepeatedWordsIgnoreCase"] = "Ignore case repeated words",
            ["UneditedSegments"] = "Check for unedited fuzzy matches",
            ["UneditedSegmentsSelectedFuzzyOnly"] = "Confirmed segments only",
            ["UneditedNotConfirmed"] = "All segments",
            ["OnlyCheckIfMatchScoresBelow"] = "Only check if match scores below",

            // Punctuation
            ["CheckPunctuationDifferences"] = "Check source and target end in same punctuation",
            ["CheckSpanishPunctuation"] = "Check spanish punctuation",
            ["CheckPunctuationSpace"] = "Check unintentional spaces before",
            ["PunctuationSpacesFrench"] = "French compliant check",
            ["CheckMultipleSpaces"] = "Check for multiple spaces",
            ["CheckMultipleDots"] = "Check for multiple dots",
            ["Ignore3Dots"] = "Ignore ellipsis dots",
            ["ExtraEndSpace"] = "Check for extra space at end of target segment",
            ["CheckInitialCaps"] = "Check capitalization of initial letters",
            ["CheckGlobalCaps"] = "Check consistency of global capitalization",
            ["CheckBrackets"] = "Check brackets",

            // Numbers
            ["CheckNumbers"] = "Check numbers",
            ["CheckDates"] = "Check dates",
            ["CheckTimes"] = "Check times",
            ["CheckMeasurements"] = "Check measurements and currencies",

            // Word List
            ["CheckWordList"] = "Check word list",
            ["WordListIgnoreCase"] = "Ignore case",
            ["WordListWholeWord"] = "Search whole words only",

            // Regular Expressions
            ["CheckRegEx"] = "Search regular expressions",
            ["RegExRules"] = "Search regular expressions",
            // ["RegExRule.IgnoreCase"] = "Ignore case", // Only if present in XML

            // Trademark Check
            ["CheckTrademarks"] = "Check trademark characters",

            // Length Verification
            ["CheckAbsoluteLength"] = "Check length limitation",
            ["AbsoluteCharCountValue"] = "Check if target segments are longer than character count",
            ["CheckAllContexts"] = "Check all segment contexts",
            ["AbsoluteLengthElements"] =
                "Check the following contexts only",
            ["CheckFilterLengthLimit"] = "Check if target segments are within file specific limits",
        };
    }
}