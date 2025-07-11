using System.Collections.Generic;
using QATracker.Components.SettingsProvider.Verifiers.Interface;

namespace QATracker.Components.SettingsProvider.Verifiers;

internal class NumberVerificationSettings : IVerificationSettings
{
    public List<string> CategoriesList { get; set; } = [Constants.NumberVerifierSettingsCategory];
    public Dictionary<string, object> DefaultSubcategoryValuesMap { get; set; } = new()
    {
        // Number reporting options
        ["ReportAddedNumbers"] = true,
        ["ReportRemovedNumbers"] = true,
        ["ReportModifiedNumbers"] = true,
        ["ReportModifiedAlphanumerics"] = true,
        ["ReportNumberFormatErrors"] = true,
        ["CheckInOrder"] = true,
        ["AlphanumericsCustomSeparator"] = false,

        // Message detail
        ["Brief"] = true,
        ["Extended"] = false,

        // Exclude filters
        ["ExcludeLockedSegments"] = false,
        ["Exclude100Percents"] = false,
        ["ExcludeTagText"] = false,
        ["ExcludeUntranslatedSegments"] = false,
        ["ExcludeDraftSegments"] = false,

        // Thousands separators (source)
        ["SourceThousandsSpace"] = true,
        ["SourceThousandsNobreakSpace"] = true,
        ["SourceThousandsThinSpace"] = true,
        ["SourceThousandsNobreakThinSpace"] = true,
        ["SourceThousandsComma"] = true,
        ["SourceThousandsPeriod"] = true,
        ["SourceNoSeparator"] = true,
        ["SourceThousandsCustom"] = true,
        ["SourceThousandsCustomSeparator"] = "",

        // Thousands separators (target)
        ["TargetThousandsSpace"] = true,
        ["TargetThousandsNobreakSpace"] = true,
        ["TargetThousandsThinSpace"] = true,
        ["TargetThousandsNobreakThinSpace"] = true,
        ["TargetThousandsComma"] = true,
        ["TargetThousandsPeriod"] = true,
        ["TargetNoSeparator"] = true,
        ["TargetThousandsCustom"] = true,
        ["TargetThousandsCustomSeparator"] = "",

        // Decimal separators (source)
        ["SourceDecimalComma"] = true,
        ["SourceDecimalPeriod"] = true,
        ["SourceDecimalCustom"] = false,
        ["SourceDecimalCustomSeparator"] = "",

        // Decimal separators (target)
        ["TargetDecimalComma"] = true,
        ["TargetDecimalPeriod"] = true,
        ["TargetDecimalCustom"] = false,
        ["TargetDecimalCustomSeparator"] = "",

        // Miscellaneous
        ["SourceOmitLeadingZero"] = true,
        ["TargetOmitLeadingZero"] = false,

        // Localization control
        ["RequireLocalizations"] = false,
        ["AllowLocalizations"] = true,
        ["PreventLocalizations"] = false
    };
    public string Id { get; set; } = Constants.NumberVerifierSettings;
    public string Name { get; set; } = Constants.NumberVerifierName;

    public Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
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
        ["SourceOmitLeadingZero"] = "Omit leading zero",
        ["TargetOmitLeadingZero"] = "Omit leading zero",

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

    public Dictionary<string, List<string>> SubcategoriesMap { get; set; } = new()
    {
        [Constants.NumberVerifierSettingsCategory] = [ 
            // Number reporting options
            "Report added numbers",
            "Report removed numbers",
            "Report modified numbers",
            "Report modified alphanumerics",
            "Report number format errors",
    
            // Other checks
            "Consider the order of the numbers in the segment pair",
            "Alphanumeric custom separators",
    
            // Exclude filters
            "Exclude locked segments",
            "Exclude 100% matches",
            "Exclude tag text",
            "Exclude untranslated segments",
            "Exclude draft segments",
    
            // Message detail
            "Brief",
            "Extended",
    
            // Localization control
            "Require localizations",
            "Allow localizations",
            "Prevent localizations",
    
            // Thousands separators
            "Space",
            "No‑break space",
            "Thin space",
            "Narrow no‑break space",
            "Comma",
            "Period",
            "No Separator",
            "Custom separator",
    
            // Decimal separators
            "Comma",
            "Period",
            "Custom separator",
    
            // Miscellaneous
            "Omit leading zero"]
    };
    public Dictionary<string, Dictionary<string, string>> UiStringToSdlprojStringMap { get; set; } = new()
    {
        [Constants.NumberVerifierSettingsCategory] = new()
        {
            // Number reporting options
            ["Report added numbers"] = "ReportAddedNumbers",
            ["Report removed numbers"] = "ReportRemovedNumbers",
            ["Report modified numbers"] = "ReportModifiedNumbers",
            ["Report modified alphanumerics"] = "ReportModifiedAlphanumerics",
            ["Report number format errors"] = "ReportNumberFormatErrors",

            // Other checks
            ["Consider the order of the numbers in the segment pair"] = "CheckInOrder",
            ["Alphanumeric custom separators"] = "AlphanumericsCustomSeparator",

            // Exclude filters
            ["Exclude locked segments"] = "ExcludeLockedSegments",
            ["Exclude 100% matches"] = "Exclude100Percents",
            ["Exclude tag text"] = "ExcludeTagText",
            ["Exclude untranslated segments"] = "ExcludeUntranslatedSegments",
            ["Exclude draft segments"] = "ExcludeDraftSegments",

            // Message detail
            ["Brief"] = "Brief",
            ["Extended"] = "Extended",

            // Localization control
            ["Require localizations"] = "RequireLocalizations",
            ["Allow localizations"] = "AllowLocalizations",
            ["Prevent localizations"] = "PreventLocalizations",

            // Thousands separators (source)
            ["Space"] = "SourceThousandsSpace",
            ["No‑break space"] = "SourceThousandsNobreakSpace",
            ["Thin space"] = "SourceThousandsThinSpace",
            ["Narrow no‑break space"] = "SourceThousandsNobreakThinSpace",
            ["Comma"] = "SourceThousandsComma",
            ["Period"] = "SourceThousandsPeriod",
            ["No Separator"] = "SourceNoSeparator",
            ["Custom separator"] = "SourceThousandsCustom",

            // Thousands separators (target)
            // These are mapped the same as source, but you may need to distinguish in your code
            // If you need to distinguish, use a prefix or suffix, e.g. "TargetThousandsSpace"
            // For demonstration, here's how you could do it:
            ["Target Space"] = "TargetThousandsSpace",
            ["Target No‑break space"] = "TargetThousandsNobreakSpace",
            ["Target Thin space"] = "TargetThousandsThinSpace",
            ["Target Narrow no‑break space"] = "TargetThousandsNobreakThinSpace",
            ["Target Comma"] = "TargetThousandsComma",
            ["Target Period"] = "TargetThousandsPeriod",
            ["Target No Separator"] = "TargetNoSeparator",
            ["Target Custom separator"] = "TargetThousandsCustom",

            // Decimal separators (source)
            ["Comma"] = "SourceDecimalComma",
            ["Period"] = "SourceDecimalPeriod",
            ["Custom separator"] = "SourceDecimalCustom",

            // Decimal separators (target)
            ["Target Comma"] = "TargetDecimalComma",
            ["Target Period"] = "TargetDecimalPeriod",
            ["Target Custom separator"] = "TargetDecimalCustom",

            // Miscellaneous
            ["Omit leading zero"] = "SourceOmitLeadingZero",
            ["Target Omit leading zero"] = "TargetOmitLeadingZero"
        }
    };
}