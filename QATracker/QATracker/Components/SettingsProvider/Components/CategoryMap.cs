using System.Collections.Generic;
using System.ComponentModel;

namespace QATracker.Components.SettingsProvider.Components
{
    public static class CategoryMap
    {
       
        public static readonly Dictionary<string, object> DefaultSubcategoryValuesMap = new()
        {
            // Forgotten and empty translation
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

            // Tag Verifier (Common)
            ["AddedTagsErrorLevel"] = true,
            ["DeletedTagsErrorLevel"] = true,
            ["TagOrderChangedErrorLevel"] = true,
            ["GhostTagErrorLevel"] = true,
            ["SpaceAroundTagsErrorLevel"] = true,
            ["CheckForIncludeTagId"] = false,
            ["IgnoreFormattingTags"] = true,
            ["IgnoreLockedSegments"] = false,
            ["TreatNonBreakingSpaceAsSpace"] = false,

            // Term Verifier (all default to false)
            ["CheckNonUsage"] = false,
            ["CheckForbidden"] = false,
            ["CheckWithoutTarget"] = false,
            ["ExcludePerfectMatch"] = false,
            ["ExcludeExactMatch"] = false,
            ["ExcludeFuzzyMatch"] = false,
            ["IgnoreLockedContent"] = false,
            ["EnableTwoLetterTermRecognition"] = false,

        };

        public static readonly Dictionary<string, Dictionary<string, string>> UiStringToSdlprojStringMap = new()
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
            },


            // Tag Verifier (Common)
            [Constants.Common] = new()
            {
                ["Tags added"] = "AddedTagsErrorLevel",
                ["Tags deleted"] = "DeletedTagsErrorLevel",
                ["Tag order change"] = "TagOrderChangedErrorLevel",
                ["Ghost tags"] = "GhostTagErrorLevel",
                ["Spacing around tags"] = "SpaceAroundTagsErrorLevel",
                ["Ignore formatting tags"] = "IgnoreFormattingTags",
                ["Ignore locked segments"] = "IgnoreLockedSegments",
                ["Ignore difference between normal and non-breaking space"] = "TreatNonBreakingSpaceAsSpace",
                ["Check for tag id mismatch"] = "CheckForIncludeTagId"
            },

            // Term Verifier (SettingsTermVerifier)
            [Constants.VerificationSettings] = new()
            {
                ["Check for possible non-usage of the target terms"] = "CheckNonUsage",
                ["Check for terms which may have been set as forbidden"] = "CheckForbidden",
                ["Check for terms without the target term equivalent"] = "CheckWithoutTarget",
                ["Exclude PerfectMatch units"] = "ExcludePerfectMatch",
                ["Exclude exact matches"] = "ExcludeExactMatch",
                ["Exclude fuzzy matches down to"] = "ExcludeFuzzyMatch",
                ["Ignore locked segments"] = "IgnoreLockedContent",
                ["Enable recognition of two-letter terms"] = "EnableTwoLetterTermRecognition",
                ["Forbidden term definitions"] = "ForbiddenDefValues",
                ["Forbidden term definition 0"] = "ForbiddenDefValues0",
                ["Forbidden term definition 1"] = "ForbiddenDefValues1",
                ["Forbidden term definition 2"] = "ForbiddenDefValues2",
                ["Forbidden term definition 3"] = "ForbiddenDefValues3",
                ["Forbidden term definition 4"] = "ForbiddenDefValues4",
                ["Forbidden term definition 5"] = "ForbiddenDefValues5",
            }
        };

        public static readonly Dictionary<string, string> SdlprojStringToUiStringMap = new()
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

            // Tag Verifier (Common)
            ["AddedTagsErrorLevel"] = "Tags added",
            ["DeletedTagsErrorLevel"] = "Tags deleted",
            ["TagOrderChangedErrorLevel"] = "Tag order change",
            ["GhostTagErrorLevel"] = "Ghost tags",
            ["SpaceAroundTagsErrorLevel"] = "Spacing around tags",
            ["IgnoreFormattingTags"] = "Ignore formatting tags",
            ["IgnoreLockedSegments"] = "Ignore locked segments",
            ["TreatNonBreakingSpaceAsSpace"] = "Ignore difference between normal and non-breaking space",
            ["CheckForIncludeTagId"] = "Check for tag id mismatch",

            // Term Verifier (SettingsTermVerifier)
            ["CheckNonUsage"] = "Check for possible non-usage of the target terms",
            ["CheckForbidden"] = "Check for forbidden terms",
            ["CheckWithoutTarget"] = "Check for terms without the target term equivalent",
            ["ExcludePerfectMatch"] = "Exclude PerfectMatch units",
            ["ExcludeExactMatch"] = "Exclude exact matches",
            ["ExcludeFuzzyMatch"] = "Exclude fuzzy matches down to",
            ["IgnoreLockedContent"] = "Ignore locked content",
            ["EnableTwoLetterTermRecognition"] = "Enable recognition of two-letter terms",
            ["ForbiddenDefValues"] = "Forbidden term definitions",
            ["ForbiddenDefValues0"] = "Forbidden term definition 0",
            ["ForbiddenDefValues1"] = "Forbidden term definition 1",
            ["ForbiddenDefValues2"] = "Forbidden term definition 2",
            ["ForbiddenDefValues3"] = "Forbidden term definition 3",
            ["ForbiddenDefValues4"] = "Forbidden term definition 4",
            ["ForbiddenDefValues5"] = "Forbidden term definition 5",
        };


        public static Dictionary<string, List<string>> SubcategoriesMap { get; set; } = new()
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
            ["Common"] =
            [
                "Tags added",
                "Tags deleted",
                "Tag order change",
                "Ghost tags",
                "Spacing around tags",
                "Check for tag id mismatch",
                "Ignore formatting tags",
                "Ignore locked segments",
                "Ignore difference between normal and non-breaking space"
            ],
            ["Verification Settings"] =
            [
                "Check for possible non-usage of the target terms",
                "Check for terms which may have been set as forbidden",
                "Check for terms without the target term equivalent",
                "Exclude PerfectMatch units",
                "Exclude exact matches",
                "Exclude fuzzy matches down to",
                "Ignore locked segments",
                "Enable recognition of two-letter terms"
            ]
        };

        public static Dictionary<string, List<string>> VerifierCategoriesMap { get; set; } = new()
        {
            [Constants.SettingsTagVerifier] = [Constants.Common],
            [Constants.SettingsTermVerifier] = [Constants.VerificationSettings],
            [Constants.QaVerificationSettings] =
            [
                Constants.SegmentsVerification,
                Constants.SegmentsToExclude,
                Constants.Inconsistencies,
                Constants.Punctuation,
                Constants.Numbers,
                Constants.WordList,
                Constants.RegularExpressions,
                Constants.TrademarkCheck,
                Constants.LengthVerification,
            ],
            [Constants.NumberVerifierSettings] = []
        };

        public static Dictionary<string, string> VerifierNamesMap { get; set; } = new()
        {
            { Constants.SettingsTagVerifier, "Tag Verifier" },
            { Constants.SettingsTermVerifier, "Term Verifier" },
            { Constants.QaVerificationSettings, "QA Verification Settings" },
            { Constants.NumberVerifierSettings, "Trados Number Verifier" }
        };
    }
}