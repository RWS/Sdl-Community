using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;
using VerifyFilesAuditReport.Components.SettingsProvider.Verifiers.BaseClass;
using VerifyFilesAuditReport.Components.SettingsProvider.Verifiers.Interface;
using VerifyFilesAuditReport.Extension;

namespace VerifyFilesAuditReport.Components.SettingsProvider.Verifiers
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
                        new()
                        {
                            Name = "CheckIdenticalSegments", Value = "False",
                            Values =
                            [
                                new() { Name = "IdenticalSegmentsIgnoreTags", Value = "False" },
                                new() { Name = "IdenticalSegmentsIgnoreCase", Value = "False" },
                            ]
                        },

                        new()
                        {
                            Name = "CheckTargetShorter", Value = "False",
                            Values =
                            [
                                new() { Name = "ShorterByValue", Value = "50" }
                            ]
                        },
                        new()
                        {
                            Name = "CheckTargetLonger", Value = "False",
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
                                new() { Name = "DiffUnitWord", Value = "True" }
                            ]
                        },
                        new()
                        {
                            Name = "CheckForbiddenChar", Value = "False",
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
                        new()
                        {
                            Name = "ExcludeFuzzyMatches", Value = "False",
                            Values =
                            [
                                new() { Name = "ExcludeFuzzyMatchValue", Value = "99" }
                            ]
                        },
                        new()
                        {
                            Name = "ElementContextExclusion", Value = "False",
                            Values =
                            [
                                new() { Name = "ElementContextExclusionValue", Value = "True" }
                            ]
                        },
                        new() { Name = "ExcludeNewTrans", Value = "False" },
                        new() { Name = "ExcludeRepetition", Value = "False" },
                        new() { Name = "ExcludeConfirmed", Value = "False" },
                        new() { Name = "ExcludeLocked", Value = "True" },
                        new() { Name = "ExcludeIdentical", Value = "False" },
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
                        new()
                        {
                            Name = "CheckInconsistencies", Value = "False",
                            Values =
                            [
                                new() { Name = "InconsistenciesIgnoreTags", Value = "False" },
                                new() { Name = "InconsistenciesIgnoreCase", Value = "False" }
                            ]
                        },
                        new()
                        {
                            Name = "CheckRepeatedWords", Value = "False",
                            Values =
                            [
                                new() { Name = "RepeatedWordsNumbers", Value = "False" },
                                new() { Name = "RepeatedWordsIgnoreCase", Value = "False" },
                            ]
                        },
                        new()
                        {
                            Name = "UneditedSegments", Value = "False",
                            Values =
                            [
                                new() { Name = "UneditedConfirmed", Value = "False" },
                                new() { Name = "UneditedNotConfirmed", Value = "False" },
                                new()
                                {
                                    Name = "UneditedSegmentsSelectedFuzzyOnly", Value = "False",
                                    Values =
                                    [
                                        new() { Name = "UneditedConfirmedLevel", Value = "99" }
                                    ]
                                }
                            ]
                        }
                    ],
                },
                new()
                {
                    Name = Constants.Punctuation,
                    Values =
                    [
                        new() { Name = "CheckPunctuationDifferences", Value = "False" },
                        new() { Name = "CheckSpanishPunctuation", Value = "False" },
                        new()
                        {
                            Name = "CheckPunctuationSpace", Value = "False",
                            Values =
                            [
                                new() { Name = "PunctuationSpaceCharsValue", Value = ":!?;" },
                                new() { Name = "PunctuationSpacesFrench", Value = "False" },
                            ]
                        },

                        new() { Name = "CheckMultipleSpaces", Value = "False" },
                        new()
                        {
                            Name = "CheckMultipleDots", Value = "False",
                            Values =
                            [
                                new() { Name = "Ignore3Dots", Value = "False" },
                            ]
                        },
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
                        new()
                        {
                            Name = "CheckWordList", Value = "False",
                            Values =
                            [
                                new() { Name = "WordListIgnoreCase", Value = "False" },
                                new() { Name = "WordListWholeWord", Value = "False" },
                                new()
                                {
                                    Name = "WrongWordPairs", Value = null,
                                    Values = []
                                },
                            ]
                        },

                    ],
                },
                new()
                {
                    Name = Constants.RegularExpressions,
                    Values =
                    [
                        new()
                        {
                            Name = "CheckRegEx", Value = "False",
                            Values =
                            [
                                new() { Name = "RegExRules", Value = "N/A" }
                            ]
                        },

                    ],
                },
                new()
                {
                    Name = Constants.TrademarkCheck,
                    Values =
                    [
                        new()
                        {
                            Name = "CheckTrademarks", Value = "False",
                            Values =
                            [
                                new() { Name = "TrademarksSymbols", Value = "N/A" }
                            ]
                        }
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

        private void MakeQaCheckerSpecificChanges()
        {
            EnhanceSegmentSizeCheckUi();
            EnhanceElementContextExclusionValueUi();
            EnhanceWordListUi();
            EnhanceRegexExpressionsUi();
            EnhanceTrademarkListUi();
        }

        private void EnhanceTrademarkListUi()
        {
            var trademarkNode = this["TrademarksSymbols"];

            var trademarks = string.Join(" ", trademarkNode.Values.Select(v => v.Value));
            trademarkNode.Value = trademarkNode.Values.Count == 0 ? "N/A" : trademarks;

            trademarkNode.Values = null;
        }

        private void EnhanceRegexExpressionsUi()
        {
            var regexRuleNode = this["RegExRules"];
            ParseXmlToRegexValues(regexRuleNode.Values);

            regexRuleNode.Value = regexRuleNode.Values.Count == 0 ? "N/A" : "";
        }

        private void ParseXmlToRegexValues(IEnumerable<VerificationSettingsTreeNode> regexNodeValues)
        {
            var regexList = regexNodeValues.Where(v => v.Name.EndsWithDigits());
            foreach (var pair in regexList)
            {
                if (string.IsNullOrWhiteSpace(pair.Value))
                    continue;

                try
                {
                    var xml = System.Xml.Linq.XElement.Parse(pair.Value);

                    // Find the RegExRule element, regardless of namespace
                    var regexRule = xml.Elements().FirstOrDefault(e => e.Name.LocalName == "RegExRule");
                    if (regexRule != null)
                    {
                        // Create subelements for each piece of info
                        var description = regexRule.Elements().FirstOrDefault(e => e.Name.LocalName == "Description")?.Value;
                        var ignoreCase = regexRule.Elements().FirstOrDefault(e => e.Name.LocalName == "IgnoreCase")?.Value;
                        var regexSource = regexRule.Elements().FirstOrDefault(e => e.Name.LocalName == "RegExSource")?.Value;
                        var regexTarget = regexRule.Elements().FirstOrDefault(e => e.Name.LocalName == "RegExTarget")?.Value;
                        var ruleCondition = regexRule.Elements().FirstOrDefault(e => e.Name.LocalName == "RuleCondition")?.Value;

                        pair.Values = new List<VerificationSettingsTreeNode>();

                        if (!string.IsNullOrEmpty(description))
                            if (!string.IsNullOrEmpty(ignoreCase))
                                pair.Values.Add(new VerificationSettingsTreeNode { Name = "IgnoreCase", Value = ignoreCase });
                        if (!string.IsNullOrEmpty(regexSource))
                            pair.Values.Add(new VerificationSettingsTreeNode { Name = "RegExSource", Value = regexSource });
                        if (!string.IsNullOrEmpty(regexTarget))
                            pair.Values.Add(new VerificationSettingsTreeNode { Name = "RegExTarget", Value = regexTarget });
                        if (!string.IsNullOrEmpty(ruleCondition))
                            pair.Values.Add(new VerificationSettingsTreeNode { Name = "RuleCondition", Value = ruleCondition });

                        // Optionally clear the string value, since info is now in subelements
                        pair.Value = description;
                        pair.Name = "";
                    }
                }
                catch
                {
                    // If parsing fails, leave the value unchanged
                }
            }
        }

        private void EnhanceWordListUi()
        {
            var wrongPairsNode = this["WrongWordPairs"];
            wrongPairsNode.Value = wrongPairsNode.Values.Count == 0 ? "N/A" : "";

            var wordList = wrongPairsNode.Values.Where(v => v.Name.EndsWithDigits());
            foreach (var pair in wordList)
            {
                if (string.IsNullOrWhiteSpace(pair.Value))
                    continue;

                try
                {
                    var xml = System.Xml.Linq.XElement.Parse(pair.Value);

                    // Find the WrongWordDef element, regardless of namespace
                    var wrongWordDef = xml.Elements().FirstOrDefault(e => e.Name.LocalName == "WrongWordDef");
                    if (wrongWordDef != null)
                    {
                        var wrongWord = wrongWordDef.Elements().FirstOrDefault(e => e.Name.LocalName == "WrongWord")?.Value
                                        ?? wrongWordDef.Elements().FirstOrDefault(e => e.Name.LocalName == "_WrongWord")?.Value;
                        var correctWord = wrongWordDef.Elements().FirstOrDefault(e => e.Name.LocalName == "CorrectWord")?.Value
                                          ?? wrongWordDef.Elements().FirstOrDefault(e => e.Name.LocalName == "_CorrectWord")?.Value;

                        if (!string.IsNullOrEmpty(wrongWord) && !string.IsNullOrEmpty(correctWord))
                        {
                            pair.Value = $"({wrongWord}, {correctWord})";
                            pair.Name = "";
                        }
                    }
                }
                catch
                {
                    // If parsing fails, leave the value unchanged
                }
            }
        }

        private void EnhanceElementContextExclusionValueUi()
        {
            var elementExclusion = this["ElementContextExclusion"];
            var values = elementExclusion.Values[0].Values;
            elementExclusion.Values = !values.Any() ? [new() { Name = "Values", Value = "N/A" }] : values;
        }

        private void EnhanceSegmentSizeCheckUi()
        {
            var diffUnitWord = this["DiffUnitWord"];
            var wordsOrChars = diffUnitWord?.Value == "True" ? "words" : "characters";

            var shorterBy = this["CheckTargetShorter"];
            var longerBy = this["CheckTargetLonger"];

            var minimalWordCountValue = this["MinimalWordCountValue"];

            if (minimalWordCountValue is null) return;

            var minimalWordCountName = Regex.Replace(SettingIdToUiStringMap["MinimalWordCountValue"], "#",
                minimalWordCountValue.Value);
            minimalWordCountName = Regex.Replace(minimalWordCountName, "words", wordsOrChars);

            minimalWordCountValue.Name = minimalWordCountName;
            minimalWordCountValue.Value = shorterBy.Value == "True" || longerBy.Value == "True" ? "True" : "False";

            minimalWordCountValue.Values = null;
        }

        public override VerificationSettingsTreeNode ToSettingsValue()
        {
            MakeQaCheckerSpecificChanges();
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
            ["UneditedSegmentsSelectedFuzzyOnly"] = "Only check if match scores below",
            ["UneditedNotConfirmed"] = "All segments",
            ["UneditedConfirmed"] = "Confirmed segments only",
            ["UneditedConfirmedLevel"] = "Level",
            ["OnlyCheckIfMatchScoresBelow"] = "Only check if match scores below",

            // Punctuation
            ["CheckPunctuationDifferences"] = "Check source and target end in same punctuation",
            ["CheckSpanishPunctuation"] = "Check spanish punctuation",
            ["CheckPunctuationSpace"] = "Check unintentional spaces before",
            ["PunctuationSpaceCharsValue"] = "Characters",
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
            ["WrongWordPairs"] = "Wrong word pairs",
            ["WrongWordPairsCount"] = "Wrong word pairs count",

            // Regular Expressions
            ["CheckRegEx"] = "Search regular expressions",
            ["RegExRules"] = "Regular expressions",
            // ["RegExRule.IgnoreCase"] = "Ignore case", // Only if present in XML

            // Trademark Check
            ["CheckTrademarks"] = "Check trademark characters",
            ["TrademarksSymbols"] = "Symbols",

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