using System.Collections.Generic;
using System.Linq;
using CaptureQARuleState.Components.SettingsProvider.Model;
using CaptureQARuleState.Components.SettingsProvider.Verifiers.BaseClass;
using CaptureQARuleState.Components.SettingsProvider.Verifiers.Interface;

namespace CaptureQARuleState.Components.SettingsProvider.Verifiers
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
                        new() { Name = "TermbaseName", Value = "" },
                        new() { Name = "CheckNonUsage", Value = "False" },
                        new()
                        {
                            Name = "CheckForbidden", Value = "False",
                            Values =
                            [
                                new() { Name = "ForbiddenDefValues", Value = "False" }
                            ]
                        },
                        new() { Name = "CheckWithoutTarget", Value = "False" },
                        new() { Name = "MinimumMatch", Value = "70" },
                        new() { Name = "SearchDepth", Value = "200" },
                        new() { Name = "RepetitionThreshold", Value = "10" },
                        new() { Name = "ExcludePerfectMatch", Value = "False" },
                        new() { Name = "ExcludeExactMatch", Value = "False" },
                        new()
                        {
                            Name = "ExcludeFuzzyMatch", Value = "False",
                            Values =
                            [
                                new() { Name = "FuzzyValue", Value = "99" }
                            ]
                        },
                        new() { Name = "IgnoreLockedContent", Value = "False" },
                        new() { Name = "EnableTwoLetterTermRecognition", Value = "False" },
                    ]
                }
            ];
        }

        public override VerificationSettingsTreeNode ToSettingsValue()
        {
            MakeTermVerificationSpecificChanges();
            return base.ToSettingsValue();
        }

        private void MakeTermVerificationSpecificChanges()
        {
            EnhanceForbiddenTermsListUi();
        }

        private void EnhanceForbiddenTermsListUi()
        {
            var forbiddenTermsList = this["ForbiddenDefValues"].Values;
            foreach (var term in forbiddenTermsList.Where(term => !string.IsNullOrWhiteSpace(term.Value)))
            {
                try
                {
                    var xml = System.Xml.Linq.XElement.Parse(term.Value);

                    var forbiddenDef = xml.Elements().FirstOrDefault(e => e.Name.LocalName == "ForbiddenDefinition");
                    if (forbiddenDef == null) continue;

                    var picklistName = forbiddenDef.Elements().FirstOrDefault(e => e.Name.LocalName == "PicklistName")?.Value;
                    var picklistValue = forbiddenDef.Elements().FirstOrDefault(e => e.Name.LocalName == "PicklistValue")?.Value;

                    term.Value = $"{picklistName}: {picklistValue}";
                    term.Name = "";
                }
                catch
                {
                    // If parsing fails, leave the value unchanged
                }
            }

            var checkForbidden = this["CheckForbidden"];
            checkForbidden.Values = forbiddenTermsList;
        }

        public override Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
        {
            ["TermbaseName"] = "Termbase name",
            ["CheckNonUsage"] = "Check for possible non-usage of the target terms",
            ["CheckForbidden"] = "Check for terms which may have been set as forbidden",
            ["CheckWithoutTarget"] = "Check for terms without the target term equivalent",
            ["MinimumMatch"] = "Minimum match value",
            ["SearchDepth"] = "Search depth",
            ["RepetitionThreshold"] = "Term repetition threshold",
            ["ExcludePerfectMatch"] = "Exclude PerfectMatch units",
            ["ExcludeExactMatch"] = "Exclude exact matches",
            ["ExcludeFuzzyMatch"] = "Exclude fuzzy matches down to",
            ["FuzzyValue"] = "Value",
            ["IgnoreLockedContent"] = "Ignore locked content",
            ["EnableTwoLetterTermRecognition"] = "Enable recognition of two-letter terms",
            ["ForbiddenDefValues"] = "Forbidden term definitions"
        };
    }
}