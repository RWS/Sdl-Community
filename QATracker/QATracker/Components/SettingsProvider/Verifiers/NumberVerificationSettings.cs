using System.Collections.Generic;
using QATracker.Components.SettingsProvider.Model;
using QATracker.Components.SettingsProvider.Verifiers.BaseClass;
using QATracker.Components.SettingsProvider.Verifiers.Interface;

namespace QATracker.Components.SettingsProvider.Verifiers;

internal class NumberVerificationSettings : VerificationSettings, IVerificationSettings
{
    public NumberVerificationSettings()
    {
        Name = Constants.NumberVerifierName;
        Values =
        [
            new()
            {
                Name = Constants.NumberVerifierSettingsCategory,
                Values =
                [
                    new() { Name = "ReportAddedNumbers", Value = "True" },
                    new() { Name = "ReportRemovedNumbers", Value = "True" },
                    new() { Name = "ReportModifiedNumbers", Value = "True" },
                    new() { Name = "ReportModifiedAlphanumerics", Value = "True" },
                    new() { Name = "ReportNumberFormatErrors", Value = "True" },
                    new() { Name = "CheckInOrder", Value = "True" },
                    new() { Name = "AlphanumericsCustomSeparator", Value = "False" },
                    new() { Name = "ExcludeLockedSegments", Value = "False" },
                    new() { Name = "Exclude100Percents", Value = "False" },
                    new() { Name = "ExcludeTagText", Value = "False" },
                    new() { Name = "ExcludeUntranslatedSegments", Value = "False" },
                    new() { Name = "ExcludeDraftSegments", Value = "False" },
                    new() { Name = "Brief", Value = "True" },
                    new() { Name = "Extended", Value = "False" },
                    new() { Name = "RequireLocalizations", Value = "False" },
                    new() { Name = "AllowLocalizations", Value = "True" },
                    new() { Name = "PreventLocalizations", Value = "False" },
                    new() { Name = "SourceThousandsSpace", Value = "True" },
                    new() { Name = "SourceThousandsNobreakSpace", Value = "True" },
                    new() { Name = "SourceThousandsThinSpace", Value = "True" },
                    new() { Name = "SourceThousandsNobreakThinSpace", Value = "True" },
                    new() { Name = "SourceThousandsComma", Value = "True" },
                    new() { Name = "SourceThousandsPeriod", Value = "True" },
                    new() { Name = "SourceNoSeparator", Value = "True" },
                    new() { Name = "SourceThousandsCustom", Value = "True" },
                    new() { Name = "TargetThousandsSpace", Value = "True" },
                    new() { Name = "TargetThousandsNobreakSpace", Value = "True" },
                    new() { Name = "TargetThousandsThinSpace", Value = "True" },
                    new() { Name = "TargetThousandsNobreakThinSpace", Value = "True" },
                    new() { Name = "TargetThousandsComma", Value = "True" },
                    new() { Name = "TargetThousandsPeriod", Value = "True" },
                    new() { Name = "TargetNoSeparator", Value = "True" },
                    new() { Name = "TargetThousandsCustom", Value = "True" },
                    new() { Name = "SourceOmitLeadingZero", Value = "False" },
                    new() { Name = "TargetOmitLeadingZero", Value = "False" }
                ],
            }
        ];
    }

    public override Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
    {
        // Exclude filters
        ["ExcludeTagText"] = "Exclude tag text",
        ["ExcludeLockedSegments"] = "Exclude locked segments",
        ["Exclude100Percents"] = "Exclude 100% matches",
        ["ExcludeUntranslatedSegments"] = "Exclude untranslated segments",
        ["ExcludeDraftSegments"] = "Exclude draft segments",

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
        ["TargetThousandsCustom"] = "Custom separator",
        ["TargetThousandsCustomSeparator"] = "Custom separator",

        // Decimal separators (source)
        ["SourceDecimalComma"] = "Comma",
        ["SourceDecimalPeriod"] = "Period",
        ["SourceDecimalCustom"] = "Custom separator",
        ["SourceDecimalCustomSeparator"] = "Custom separator",

        // Decimal separators (target)
        ["TargetDecimalComma"] = "Comma",
        ["TargetDecimalPeriod"] = "Period",
        ["TargetDecimalCustom"] = "Custom separator",
        ["TargetDecimalCustomSeparator"] = "Custom separator",

        // Omit leading zero
        ["SourceOmitLeadingZero"] = "Source omit leading zero",
        ["TargetOmitLeadingZero"] = "Target omit leading zero",

        // Alphanumerics custom separator
        ["AlphanumericsCustomSeparator"] = "Alphanumeric custom separators",
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