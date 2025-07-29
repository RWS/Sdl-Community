using QATracker.Components.SettingsProvider.Model;
using QATracker.Components.SettingsProvider.Verifiers.BaseClass;
using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;
using System.Linq;

namespace QATracker.Components.SettingsProvider.Verifiers;

internal class NumberVerificationSettings : VerificationSettings, IVerificationSettings
{
    public NumberVerificationSettings()
    {
        Name = Constants.NumberVerifierName;
        Values =
        [

            new() { Name = "ReportAddedNumbers", Value = "True" },
            new() { Name = "ReportRemovedNumbers", Value = "True" },
            new() { Name = "ReportModifiedNumbers", Value = "True" },
            new() { Name = "ReportModifiedAlphanumerics", Value = "True" },
            new() { Name = "ReportNumberFormatErrors", Value = "True" },
            new() { Name = "CheckInOrder", Value = "True" },
            new()
            {
                Name = "CustomsSeparatorsAlphanumerics", Value = "True",
                Values =
                [
                    new() { Name = "AlphanumericsCustomSeparator", Value = "N/A" }
                ]
            },
            new()
            {
                Name = "Exclusions",
                Values =
                [
                    new() { Name = "ExcludeLockedSegments", Value = "False" },
                    new() { Name = "Exclude100Percents", Value = "False" },
                    new() { Name = "ExcludeTagText", Value = "False" },
                    new() { Name = "ExcludeUntranslatedSegments", Value = "False" },
                    new() { Name = "ExcludeDraftSegments", Value = "False" },
                    new() { Name = "RegexExclusionList", Value = "N/A"}
                ]
            },
            new()
            {
                Name = "Localizations",
                Values =
                [
                    new() { Name = "RequireLocalizations", Value = "False" },
                    new() { Name = "AllowLocalizations", Value = "True" },
                    new() { Name = "PreventLocalizations", Value = "False" }
                ]
            },
            new()
            {
                Name = "Thousands separators",
                Values =
                [
                    new()
                    {
                        Name = "Source", Values =
                        [
                            new() { Name = "SourceThousandsSpace", Value = "True" },
                            new() { Name = "SourceThousandsNobreakSpace", Value = "True" },
                            new() { Name = "SourceThousandsThinSpace", Value = "True" },
                            new() { Name = "SourceThousandsNobreakThinSpace", Value = "True" },
                            new() { Name = "SourceThousandsComma", Value = "True" },
                            new() { Name = "SourceThousandsPeriod", Value = "True" },
                            new() { Name = "SourceNoSeparator", Value = "True" },
                            new()
                            {
                                Name = "SourceThousandsCustom", Value = "True",
                                Values = [new() { Name = "SourceThousandsCustomSeparator", Value = "" }]
                            },
                        ]
                    },
                    new()
                    {
                        Name = "Target", Values =
                        [
                            new() { Name = "TargetThousandsSpace", Value = "True" },
                            new() { Name = "TargetThousandsNobreakSpace", Value = "True" },
                            new() { Name = "TargetThousandsThinSpace", Value = "True" },
                            new() { Name = "TargetThousandsNobreakThinSpace", Value = "True" },
                            new() { Name = "TargetThousandsComma", Value = "True" },
                            new() { Name = "TargetThousandsPeriod", Value = "True" },
                            new() { Name = "TargetNoSeparator", Value = "True" },
                            new()
                            {
                                Name = "TargetThousandsCustom", Value = "True",
                                Values =
                                [
                                    new() { Name = "TargetThousandsCustomSeparator", Value = "N/A" }
                                ]
                            }
                        ]
                    }
                ]
            },
            new()
            {
                Name = "Decimal separators",
                Values =
                [
                    new()
                    {
                        Name = "Source",
                        Values =
                        [
                            new() { Name = "SourceDecimalComma", Value = "True" },
                            new() { Name = "SourceDecimalPeriod", Value = "True" },
                            new()
                            {
                                Name = "SourceDecimalCustom", Value = "False",
                                Values =
                                [
                                    new() { Name = "SourceDecimalCustomSeparator", Value = "N/A" }
                                ]
                            },
                        ]
                    },
                    new()
                    {
                        Name = "Target",
                        Values =
                        [
                            new() { Name = "TargetDecimalComma", Value = "True" },
                            new() { Name = "TargetDecimalPeriod", Value = "True" },
                            new()
                            {
                                Name = "TargetDecimalCustom", Value = "False",
                                Values =
                                [
                                    new() { Name = "TargetDecimalCustomSeparator", Value = "N/A" }
                                ]
                            }
                        ]
                    }
                ],
            },

            new()
            {
                Name = "Miscellaneous", Values =
                [
                    new() { Name = "SourceOmitLeadingZero", Value = "False" },
                    new() { Name = "TargetOmitLeadingZero", Value = "False" }
                ]
            },

        ];
    }

    public override VerificationSettingsTreeNode ToSettingsValue()
    {
        MakeNumberVerificationSpecificChanges();
        return base.ToSettingsValue();
    }

    private void MakeNumberVerificationSpecificChanges()
    {
        EnhanceRegexExclusionListUi();
    }

    private void EnhanceRegexExclusionListUi()
    {
        var regexExclusionList = this["RegexExclusionList"];
        if (regexExclusionList == null || string.IsNullOrWhiteSpace(regexExclusionList.Value))
            return;

        try
        {
            var xml = System.Xml.Linq.XElement.Parse(regexExclusionList.Value);

            // Find the ArrayOfRegexPattern element, regardless of namespace
            var arrayOfRegexPattern = xml.Elements().FirstOrDefault(e => e.Name.LocalName == "ArrayOfRegexPattern");
            if (arrayOfRegexPattern != null)
            {
                var regexPatterns = arrayOfRegexPattern.Elements().Where(e => e.Name.LocalName == "RegexPattern");
                var valueList = new List<VerificationSettingsTreeNode>();

                foreach (var patternElem in regexPatterns)
                {
                    var name = patternElem.Elements().FirstOrDefault(e => e.Name.LocalName == "Name")?.Value;
                    var pattern = patternElem.Elements().FirstOrDefault(e => e.Name.LocalName == "Pattern")?.Value;

                    var node = new VerificationSettingsTreeNode
                    {
                        Name = name ?? string.Empty,
                        Value = pattern ?? string.Empty
                    };
                    valueList.Add(node);
                }

                regexExclusionList.Values = valueList;

                regexExclusionList.Value = null;
            }
        }
        catch
        {
            // If parsing fails, leave the value unchanged
        }
    }


    public override Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
    {
        // Exclude filters
        ["ExcludeTagText"] = "Exclude tag text",
        ["ExcludeLockedSegments"] = "Exclude locked segments",
        ["Exclude100Percents"] = "Exclude 100% matches",
        ["ExcludeUntranslatedSegments"] = "Exclude untranslated segments",
        ["ExcludeDraftSegments"] = "Exclude draft segments",
        ["RegexExclusionList"] = "Regex list",

        // Thousands separators (source)
        ["SourceThousandsSpace"] = "Space",
        ["SourceThousandsNobreakSpace"] = "No‑break space",
        ["SourceThousandsThinSpace"] = "Thin space",
        ["SourceThousandsNobreakThinSpace"] = "Narrow no‑break space",
        ["SourceThousandsComma"] = "Comma",
        ["SourceThousandsPeriod"] = "Period",
        ["SourceNoSeparator"] = "No Separator",
        ["SourceThousandsCustom"] = "Custom separator",
        ["SourceThousandsCustomSeparator"] = "Custom separator",

        // Thousands separators (target)
        ["TargetThousandsSpace"] = "Space",
        ["TargetThousandsNobreakSpace"] = "No‑break space",
        ["TargetThousandsThinSpace"] = "Thin space",
        ["TargetThousandsNobreakThinSpace"] = "Narrow no‑break space",
        ["TargetThousandsComma"] = "Comma",
        ["TargetThousandsPeriod"] = "Period",
        ["TargetNoSeparator"] = "No Separator",
        ["TargetThousandsCustom"] = "Separator",
        ["TargetThousandsCustomSeparator"] = "Separator",

        // Decimal separators (source)
        ["SourceDecimalComma"] = "Comma",
        ["SourceDecimalPeriod"] = "Period",
        ["SourceDecimalCustom"] = "Custom separator",
        ["SourceDecimalCustomSeparator"] = "Separator",

        // Decimal separators (target)
        ["TargetDecimalComma"] = "Comma",
        ["TargetDecimalPeriod"] = "Period",
        ["TargetDecimalCustom"] = "Custom separator",
        ["TargetDecimalCustomSeparator"] = "Separator",

        // Omit leading zero
        ["SourceOmitLeadingZero"] = "Source omit leading zero",
        ["TargetOmitLeadingZero"] = "Target omit leading zero",

        // Alphanumerics custom separator
        ["AlphanumericsCustomSeparator"] = "Separators",
        ["CustomsSeparatorsAlphanumerics"] = "Alphanumeric custom separators",

        // Number reporting options
        ["ReportAddedNumbers"] = "Report added numbers",
        ["ReportRemovedNumbers"] = "Report removed numbers",
        ["ReportModifiedNumbers"] = "Report modified numbers",
        ["ReportModifiedAlphanumerics"] = "Report modified alphanumerics",
        ["ReportNumberFormatErrors"] = "Report number format errors",

        // Other checks
        ["CheckInOrder"] = "Consider the order of the numbers in the segment pair",
        ["RequireLocalizations"] = "Require localizations"
    };

}