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
        Children =
    [
        new()
        {
            Name = Constants.NumberVerifierSettingsCategory,
            Values =
            [
                new() { Name = "Report added numbers", Value = "True" },
                new() { Name = "Report removed numbers", Value = "True" },
                new() { Name = "Report modified numbers", Value = "True" },
                new() { Name = "Report modified alphanumerics", Value = "True" },
                new() { Name = "Report number format errors", Value = "True" },
                new() { Name = "Consider the order of the numbers in the segment pair", Value = "True" },
                new() { Name = "Alphanumeric custom separators", Value = "False" },
                new() { Name = "Exclude locked segments", Value = "False" },
                new() { Name = "Exclude 100% matches", Value = "False" },
                new() { Name = "Exclude tag text", Value = "False" },
                new() { Name = "Exclude untranslated segments", Value = "False" },
                new() { Name = "Exclude draft segments", Value = "False" },
                new() { Name = "Brief", Value = "True" },
                new() { Name = "Extended", Value = "False" },
                new() { Name = "Require localizations", Value = "False" },
                new() { Name = "Allow localizations", Value = "True" },
                new() { Name = "Prevent localizations", Value = "False" },
                new() { Name = "Space", Value = "True" },
                new() { Name = "No‑break space", Value = "True" },
                new() { Name = "Thin space", Value = "True" },
                new() { Name = "Narrow no‑break space", Value = "True" },
                new() { Name = "Comma", Value = "True" },
                new() { Name = "Period", Value = "True" },
                new() { Name = "No Separator", Value = "True" },
                new() { Name = "Custom separator", Value = "True" },
                new() { Name = "Target Space", Value = "True" },
                new() { Name = "Target No‑break space", Value = "True" },
                new() { Name = "Target Thin space", Value = "True" },
                new() { Name = "Target Narrow no‑break space", Value = "True" },
                new() { Name = "Target Comma", Value = "True" },
                new() { Name = "Target Period", Value = "True" },
                new() { Name = "Target No Separator", Value = "True" },
                new() { Name = "Target Custom separator", Value = "True" },
                new() { Name = "Source omit leading zero", Value = "False" },
                new() { Name = "Target omit leading zero", Value = "False" }
            ],
            Children = null
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