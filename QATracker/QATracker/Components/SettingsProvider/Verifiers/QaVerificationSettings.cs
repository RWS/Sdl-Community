using QATracker.Components.SettingsProvider.Verifiers.BaseClass;
using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;

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
                        new() { Name = "Check for forgotten and empty translations", Value = "True" },
                        new() { Name = "Source and target are identical", Value = "False" },
                        new() { Name = "Ignore tags", Value = "False" },
                        new() { Name = "Ignore case", Value = "False" },
                        new()
                        {
                            Name = "Shorter by (%)", Enabled = "False",
                            Values =
                             [
                                 new() { Name = "Shorter by value", Value = "50" }
                             ]
                        },
                        new()
                        {
                            Name = "Longer by (%)", Enabled = "False",
                            Values =
                            [
                                new() { Name = "Longer by value", Value = "50" }
                            ]
                        },
                        new() { Name = "Ignore segments with fewer than # words", Value = "2" },
                        new() { Name = "Based on words", Value = "True" },
                        new() { Name = "Based on characters", Value = "" },
                        new()
                        { 
                            Name = "Check for forbidden characters", Enabled = "False" ,
                            Values = 
                            [
                                new() { Name = "Forbidden chars: ", Value = "" }
                            ]
                        },
                        
                    ],
                    
                },
                new()
                {
                    Name = Constants.SegmentsToExclude,
                    Values =
                    [
                        new() { Name = "Exclude PerfectMatch and Context Match units", Value = "True" },
                        new() { Name = "Exclude exact matches", Value = "False" },
                        new() { Name = "Exclude fuzzy matches down to", Value = "False" },
                        new() { Name = "Exclude fuzzy match value", Value = "99" },
                        new() { Name = "Element context exclusion value", Value = "True" },
                        new() { Name = "Exclude new translations", Value = "False" },
                        new() { Name = "Exclude repetitions", Value = "False" },
                        new() { Name = "Exclude confirmed translations", Value = "False" },
                        new() { Name = "Exclude locked segments", Value = "True" },
                        new() { Name = "Exclude target segment identical to source", Value = "False" },
                        new() { Name = "Check only segments which are in following contexts", Value = "False" },
                        new() { Name = "Exclusion string value", Value = "True" },

                        new() { Name = "Apply exclusion list to search for not translated segments", Value = "False" },
                        new()
                        {
                            Name = "Apply exclusion list to search for target segments identical to source",
                            Value = "False"
                        },
                        new() { Name = "Ignore case", Value = "False" },
                        new() { Name = "Exclusions use regular expressions", Value = "False" },
                        new() { Name = "Report all non-excluded segments", Value = "False" }
                    ]
                    
                },
                new()
                {
                    Name = Constants.Inconsistencies,
                    Values =
                    [
                        new() { Name = "Check for inconsistent translations", Value = "False" },
                        new() { Name = "Ignore tags", Value = "False" },
                        new() { Name = "Ignore case inconsistencies", Value = "False" },
                        new() { Name = "Check for repeated words in target", Value = "False" },
                        new() { Name = "Ignore numbers", Value = "False" },
                        new() { Name = "Ignore case repeated words", Value = "False" },
                        new() { Name = "Check for unedited fuzzy matches", Value = "False" },
                        new() { Name = "Confirmed segments only", Value = "True" },
                        new() { Name = "All segments", Value = "False" },
                        new() { Name = "Only check if match scores below", Value = "False" }
                    ],
                    
                },
                new()
                {
                    Name = Constants.Punctuation,
                    Values =
                    [
                        new() { Name = "Check source and target end in same punctuation", Value = "False" },
                        new() { Name = "Check spanish punctuation", Value = "False" },
                        new() { Name = "Check unintentional spaces before", Value = "False" },
                        new() { Name = "French compliant check", Value = "False" },
                        new() { Name = "Check for multiple spaces", Value = "False" },
                        new() { Name = "Check for multiple dots", Value = "False" },
                        new() { Name = "Ignore ellipsis dots", Value = "False" },
                        new() { Name = "Check for extra space at end of target segment", Value = "False" },
                        new() { Name = "Check capitalization of initial letters", Value = "False" },
                        new() { Name = "Check consistency of global capitalization", Value = "False" },
                        new() { Name = "Check brackets", Value = "False" }
                    ],
                    
                },
                new()
                {
                    Name = Constants.Numbers,
                    Values =
                    [
                        new() { Name = "Check numbers", Value = "False" },
                        new() { Name = "Check dates", Value = "False" },
                        new() { Name = "Check times", Value = "False" },
                        new() { Name = "Check measurements and currencies", Value = "False" }
                    ],
                    
                },
                new()
                {
                    Name = Constants.WordList,
                    Values =
                    [
                        new() { Name = "Check word list", Value = "False" },
                        new() { Name = "Ignore case", Value = "False" },
                        new() { Name = "Search whole words only", Value = "False" }
                    ],
                    
                },
                new()
                {
                    Name = Constants.RegularExpressions,
                    Values =
                    [
                        new() { Name = "Search regular expressions", Value = "False" },
                        new() { Name = "Ignore case", Value = "False" }
                    ],
                    
                },
                new()
                {
                    Name = Constants.TrademarkCheck,
                    Values =
                    [
                        new() { Name = "Check trademark characters", Value = "False" }
                    ],
                    
                },
                new()
                {
                    Name = Constants.LengthVerification,
                    Values =
                    [
                        new() { Name = "Check length limitation", Value = "True" },
                        new() { Name = "Check if target segments are within file specific limits", Value = "True" },
                        new() { Name = "Check if target segments are longer than character count", Value = "50" },
                        new() { Name = "Check all segment contexts", Value = "True" },
                        new() { Name = "Check the following contexts only", Value = "" }
                    ],
                    
                }
            ];
            
        }

        public override Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
        {
            // Segments Verification
            ["CheckIdenticalSegments"] = "Source and target are identical",
            ["IdenticalSegmentsIgnoreTags"] = "Ignore tags",
            ["IdenticalSegmentsIgnoreCase"] = "Ignore case",
            ["CheckTargetShorter"] = "Shorter by (%)",
            ["CheckTargetLonger"] = "Longer by (%)",
            ["DiffUnitWord"] = "Based on words",
            ["MinimalWordCountValue"] = "Ignore segments with fewer than # words",
            ["CheckUntranslatedSegments"] = "Check for forgotten and empty translations",
            ["CheckForbiddenChar"] = "Check for forbidden characters",
            ["ShorterByValue"] = "Shorter by value",
            ["LongerByValue"] = "Longer by value",
            ["ForbiddenCharsValue"] = "Forbidden chars: ",


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
            ["InconsistenciesIgnoreCase"] = "Ignore case inconsistencies",
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