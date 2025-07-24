using QATracker.Components.SettingsProvider.Verifiers.BaseClass;
using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers
{
    public class TermVerificationSettings : VerificationSettings, IVerificationSettings
    {
        public TermVerificationSettings()
        {
            Name = Constants.TermVerifierName;
            Values =
            [
                new()
                {
                    Name = "Verification Settings",
                    Values =
                    [
                        new() { Name = "CheckNonUsage", Value = "False" },
                        new() { Name = "CheckForbidden", Value = "False" },
                        new() { Name = "CheckWithoutTarget", Value = "False" },
                        new() { Name = "ExcludePerfectMatch", Value = "False" },
                        new() { Name = "ExcludeExactMatch", Value = "False" },
                        new() { Name = "ExcludeFuzzyMatch", Value = "False" },
                        new() { Name = "IgnoreLockedContent", Value = "False" },
                        new() { Name = "EnableTwoLetterTermRecognition", Value = "False" },
                        new() { Name = "ForbiddenDefValues", Value = "False" },
                        new()
                        {
                            Name = "ForbiddenDefValues",
                            Values =
                            [
                                new() { Name = "ForbiddenDefValues0", Value = null },
                                new() { Name = "ForbiddenDefValues1", Value = null },
                                new() { Name = "ForbiddenDefValues2", Value = null },
                                new() { Name = "ForbiddenDefValues3", Value = null },
                                new() { Name = "ForbiddenDefValues4", Value = null },
                                new() { Name = "ForbiddenDefValues5", Value = null }
                            ],
                        }
                    ]
                }
            ];
        }

        public override Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
        {
            ["CheckNonUsage"] = "Check for possible non-usage of the target terms",
            ["CheckForbidden"] = "Check for terms which may have been set as forbidden",
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
    }
}