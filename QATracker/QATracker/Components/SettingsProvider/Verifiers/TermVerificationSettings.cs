using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers
{
    public class TermVerificationSettings : IVerificationSettings
    {
        public List<string> CategoriesList { get; set; } = [Constants.VerificationSettings];

        public Dictionary<string, object> DefaultSubcategoryValuesMap { get; set; } = new()
        {
            ["CheckNonUsage"] = false,
            ["CheckForbidden"] = false,
            ["CheckWithoutTarget"] = false,
            ["ExcludePerfectMatch"] = false,
            ["ExcludeExactMatch"] = false,
            ["ExcludeFuzzyMatch"] = false,
            ["IgnoreLockedContent"] = false,
            ["EnableTwoLetterTermRecognition"] = false
        };

        public string Id { get; set; } = Constants.SettingsTermVerifier;

        public string Name { get; set; } = Constants.TermVerifierName;

        public Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
        {
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
            ["ForbiddenDefValues5"] = "Forbidden term definition 5"
        };

        public Dictionary<string, List<string>> SubcategoriesMap { get; set; } = new()
        {
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

        public Dictionary<string, Dictionary<string, string>> UiStringToSdlprojStringMap { get; set; } = new()
        {
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
    }
}