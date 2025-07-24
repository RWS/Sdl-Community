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
                        new() { Name = "Check for possible non-usage of the target terms", Value = "False" },
                        new() { Name = "Check for terms which may have been set as forbidden", Value = "False" },
                        new() { Name = "Check for terms without the target term equivalent", Value = "False" },
                        new() { Name = "Exclude PerfectMatch units", Value = "False" },
                        new() { Name = "Exclude exact matches", Value = "False" },
                        new() { Name = "Exclude fuzzy matches down to", Value = "False" },
                        new() { Name = "Ignore locked segments", Value = "False" },
                        new() { Name = "Enable recognition of two-letter terms", Value = "False" },
                        new() { Name = "Forbidden term definitions", Value = "False" },
                        new()
                        {
                            Name = "Forbidden term definitions",
                            Values =
                            [
                                new() { Name = "Forbidden term definition 0", Value = null },
                                new() { Name = "Forbidden term definition 1", Value = null },
                                new() { Name = "Forbidden term definition 2", Value = null },
                                new() { Name = "Forbidden term definition 3", Value = null },
                                new() { Name = "Forbidden term definition 4", Value = null },
                                new() { Name = "Forbidden term definition 5", Value = null }
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