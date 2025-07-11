using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers
{
    public class QaVerificationSettings : IVerificationSettings
    {
        
        public List<string> CategoriesList { get; set; } =
        [
            Constants.SegmentsVerification,
            Constants.SegmentsToExclude,
            Constants.Inconsistencies,
            Constants.Punctuation,
            Constants.Numbers,
            Constants.WordList,
            Constants.RegularExpressions,
            Constants.TrademarkCheck,
            Constants.LengthVerification
        ];

        public Dictionary<string, object> DefaultSubcategoryValuesMap { get; set; } = new()
        { // Forgotten and empty translation
            ["CheckUntranslatedSegments"] = true,

            // Compare source and target segments
            ["CheckIdenticalSegments"] = false,
            ["IdenticalSegmentsIgnoreTags"] = false,
            ["IdenticalSegmentsIgnoreCase"] = false,

            // Shorter/Longer by (%)
            ["CheckTargetShorter"] = false,
            ["CheckTargetLonger"] = false,

            ["DiffUnitWord"] = true,

            // Ignore segments with fewer than # words
            ["MinimalWordCountValue"] = 2, // Default value in the UI

            // Check target segments for forbidden characters
            ["CheckForbiddenChar"] = false,

            // Segment exclusion
            ["ExcludePerfectMatchSegments"] = true,
            ["ExcludeExactMatches"] = false,
            ["ExcludeFuzzyMatches"] = false,
            ["ExcludeNewTrans"] = false,
            ["ExcludeRepetition"] = false,
            ["ExcludeConfirmed"] = false,
            ["ExcludeLocked"] = true,
            ["ExcludeIdentical"] = false,
            ["ElementContextExclusion"] = false,
            ["ExclusionStringUntranslated"] = false,
            ["ExclusionStringIdentical"] = false,
            ["ExclusionStringIgnoreCase"] = false,
            ["ExclusionStringRegEx"] = false,
            ["ReportAllNonExcluded"] = false,

            // Inconsistencies
            ["CheckInconsistencies"] = false,
            ["InconsistenciesIgnoreTags"] = false,
            ["InconsistenciesIgnoreCase"] = false,
            ["CheckRepeatedWords"] = false,
            ["RepeatedWordsNumbers"] = false,
            ["RepeatedWordsIgnoreCase"] = false,
            ["UneditedSegments"] = false,
            ["UneditedSegmentsSelectedFuzzyOnly"] = true,   // "Confirmed segments only" is selected by default
            ["UneditedNotConfirmed"] = false,               // "All segments" is not selected
            ["OnlyCheckIfMatchScoresBelow"] = false,

            // Punctuation (all default to false)
            ["CheckPunctuationDifferences"] = false,
            ["CheckSpanishPunctuation"] = false,
            ["CheckPunctuationSpace"] = false,
            ["PunctuationSpacesFrench"] = false,
            ["CheckMultipleSpaces"] = false,
            ["CheckMultipleDots"] = false,
            ["Ignore3Dots"] = false,
            ["ExtraEndSpace"] = false,
            ["CheckInitialCaps"] = false,
            ["CheckGlobalCaps"] = false,
            ["CheckBrackets"] = false,

            // Numbers
            ["CheckNumbers"] = false,
            ["CheckDates"] = false,
            ["CheckTimes"] = false,
            ["CheckMeasurements"] = false,

            // Word List (all default to false)
            ["CheckWordList"] = false,
            ["WordListIgnoreCase"] = false,
            ["WordListWholeWord"] = false,

            // Regular Expressions
            ["CheckRegEx"] = false,
            ["RegExRules"] = "", // Default to empty string
            ["RegExRule.IgnoreCase"] = false, // Only if present in XML

            // Trademark Check
            ["CheckTrademarks"] = false,

            // Length Verification
            ["CheckAbsoluteLength"] = true,
            ["AbsoluteCharCountValue"] = 50,
            ["CheckAllContexts"] = true,
            ["AbsoluteLengthElements"] = "",
            ["CheckFilterLengthLimit"] = true,
        };

        public string Id { get; set; } = Constants.QaVerificationSettings;

        public string Name { get; set; } = Constants.QaCheckerVerifierName;

        public Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
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

            // Segment exclusion
            ["ExcludePerfectMatchSegments"] = "Exclude PerfectMatch and Context Match units",
            ["ExcludeLocked"] = "Exclude locked segments",

            ["ExcludeExactMatches"] = "Exclude exact matches",
            ["ExcludeFuzzyMatches"] = "Exclude fuzzy matches down to",
            ["ExcludeNewTrans"] = "Exclude new translations",
            ["ExcludeRepetition"] = "Exclude repetitions",
            ["ExcludeConfirmed"] = "Exclude confirmed translations",
            ["ExcludeIdentical"] = "Exclude target segment identical to source",
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
        };

        public Dictionary<string, List<string>> SubcategoriesMap { get; set; } = new()
        {
            ["Segments Verification"] =
            [
                "Check for forgotten and empty translations",
                "Source and target are identical",
                "Ignore tags",
                "Ignore case",
                "Shorter by (%)",
                "Longer by (%)",
                "Ignore segments with fewer than # words",
                "Based on words",
                "Based on characters",
                "Check for forbidden characters"
            ],
            ["Segments to Exclude"] =
            [
                "Exclude PerfectMatch and Context Match units",
                "Exclude exact matches",
                "Exclude fuzzy matches down to",
                "Exclude new translations",
                "Exclude repetitions",
                "Exclude confirmed translations",
                "Exclude locked segments",
                "Exclude target segment identical to source",
                "Check only segments which are in following contexts",
                "Apply exclusion list to search for not translated segments",
                "Apply exclusion list to search for target segments identical to source",
                "Ignore case",
                "Exclusions use regular expressions",
                "Report all non-excluded segments"
            ],
            ["Inconsistencies"] =
            [
                "Check for inconsistent translations",
                "Ignore tags",
                "Ignore case inconsistencies",
                "Check for repeated words in target",
                "Ignore numbers",
                "Ignore case repeated words",
                "Check for unedited fuzzy matches",
                "Confirmed segments only",
                "All segments",
                "Only check if match scores below"
            ],
            ["Punctuation"] =
            [
                "Check source and target end in same punctuation",
                "Check spanish punctuation",
                "Check unintentional spaces before",
                "French compliant check",
                "Check for multiple spaces",
                "Check for multiple dots",
                "Ignore ellipsis dots",
                "Check for extra space at end of target segment",
                "Check capitalization of initial letters",
                "Check consistency of global capitalization",
                "Check brackets"
            ],
            ["Numbers"] =
            [
                "Check numbers",
                "Check dates",
                "Check times",
                "Check measurements and currencies"
            ],
            ["Regular Expressions"] =
            [
                "Search regular expressions",
                "Ignore case"
            ],
            ["Word List"] =
            [
                "Check word list",
                "Ignore case",
                "Search whole words only"
            ],
            ["Trademark Check"] = ["Check trademark characters"],
            ["Length Verification"] =
            [
                "Check length limitation",
                "Check if target segments are within file specific limits",
                "Check if target segments are longer than character count",
                "Check all segment contexts",
                "Check the following contexts only"
            ],
        };

        public Dictionary<string, Dictionary<string, string>> UiStringToSdlprojStringMap { get; set; } = new()
        {
            // Segments Verification
            [Constants.SegmentsVerification] = new()
            {
                ["Check for forgotten and empty translations"] = "CheckUntranslatedSegments",
                ["Source and target are identical"] = "CheckIdenticalSegments",
                ["Ignore tags"] = "IdenticalSegmentsIgnoreTags",
                ["Ignore case"] = "IdenticalSegmentsIgnoreCase",
                ["Shorter by (%)"] = "CheckTargetShorter",
                ["Longer by (%)"] = "CheckTargetLonger",
                ["Based on words"] = "DiffUnitWord",
                ["Based on characters"] = "DiffUnitWord",
                ["Ignore segments with fewer than # words"] = "MinimalWordCountValue",
                ["Check for forbidden characters"] = "CheckForbiddenChar"
            },
            [Constants.SegmentsToExclude] = new()
            {
                ["Exclude PerfectMatch and Context Match units"] = "ExcludePerfectMatchSegments",
                ["Exclude exact matches"] = "ExcludeExactMatches",
                ["Exclude fuzzy matches down to"] = "ExcludeFuzzyMatches",
                ["Exclude new translations"] = "ExcludeNewTrans",
                ["Exclude repetitions"] = "ExcludeRepetition",
                ["Exclude confirmed translations"] = "ExcludeConfirmed",
                ["Exclude locked segments"] = "ExcludeLocked",
                ["Exclude target segment identical to source"] = "ExcludeIdentical",
                ["Check only segments which are in following contexts"] = "ElementContextExclusion",
                ["Apply exclusion list to search for not translated segments"] = "ExclusionStringUntranslated",
                ["Apply exclusion list to search for target segments identical to source"] = "ExclusionStringIdentical",
                ["Ignore case"] = "ExclusionStringIgnoreCase",
                ["Exclusions use regular expressions"] = "ExclusionStringRegEx",
                ["Report all non-excluded segments"] = "ReportAllNonExcluded"
            },

            [Constants.Inconsistencies] = new()
            {
                ["Check for inconsistent translations"] = "CheckInconsistencies",
                ["Ignore tags"] = "InconsistenciesIgnoreTags",
                ["Ignore case inconsistencies"] = "InconsistenciesIgnoreCase",
                ["Check for repeated words in target"] = "CheckRepeatedWords",
                ["Ignore numbers"] = "RepeatedWordsNumbers",
                ["Ignore case repeated words"] = "RepeatedWordsIgnoreCase",
                ["Check for unedited fuzzy matches"] = "UneditedSegments",
                ["Confirmed segments only"] = "UneditedSegmentsSelectedFuzzyOnly",
                ["All segments"] = "UneditedNotConfirmed",
                ["Only check if match scores below"] = "OnlyCheckIfMatchScoresBelow"
            },

            [Constants.Punctuation] = new()
            {
                ["Check source and target end in same punctuation"] = "CheckPunctuationDifferences",
                ["Check spanish punctuation"] = "CheckSpanishPunctuation",
                ["Check unintentional spaces before"] = "CheckPunctuationSpace",
                ["French compliant check"] = "PunctuationSpacesFrench",
                ["Check for multiple spaces"] = "CheckMultipleSpaces",
                ["Check for multiple dots"] = "CheckMultipleDots",
                ["Ignore ellipsis dots"] = "Ignore3Dots",
                ["Check for extra space at end of target segment"] = "ExtraEndSpace",
                ["Check capitalization of initial letters"] = "CheckInitialCaps",
                ["Check consistency of global capitalization"] = "CheckGlobalCaps",
                ["Check brackets"] = "CheckBrackets"
            },

            [Constants.Numbers] = new()
            {
                ["Check numbers"] = "CheckNumbers",
                ["Check dates"] = "CheckDates",
                ["Check times"] = "CheckTimes",
                ["Check measurements and currencies"] = "CheckMeasurements"
            },

            // Word List
            [Constants.WordList] = new()
            {
                ["Check word list"] = "CheckWordList",
                ["Ignore case"] = "WordListIgnoreCase",
                ["Search whole words only"] = "WordListWholeWord"
            },

            // Regular Expressions
            [Constants.RegularExpressions] = new()
            {
                ["Search regular expressions"] = "CheckRegEx",
                ["Ignore case"] = "RegExRule.IgnoreCase"
            },

            // Trademark Check
            [Constants.TrademarkCheck] = new()
            {
                ["Check trademark characters"] = "CheckTrademarks"
            },

            // Length Verification
            [Constants.LengthVerification] = new()
            {
                ["Check length limitation"] = "CheckAbsoluteLength",
                ["Check if target segments are within file specific limits"] = "CheckFilterLengthLimit",
                ["Check if target segments are longer than character count"] = "AbsoluteCharCountValue",
                ["Check all segment contexts"] = "CheckAllContexts",
                ["Check the following contexts only"] = "AbsoluteLengthElements"
            }
        };
    }
}