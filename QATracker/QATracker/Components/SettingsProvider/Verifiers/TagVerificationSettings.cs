using QATracker.Components.SettingsProvider.Model;
using QATracker.Components.SettingsProvider.Verifiers.BaseClass;
using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers
{
    public class TagVerificationSettings : VerificationSettings, IVerificationSettings
    {
        public TagVerificationSettings()
        {
            Name = Constants.TagVerifierName;
            Values =
            [
                new()
                {
                    Name = Constants.Common,
                    Values =
                    [
                        new() { Name = "Tags added", Value = "True" },
                        new() { Name = "Tags deleted", Value = "True" },
                        new() { Name = "Tag order change", Value = "True" },
                        new() { Name = "Ghost tags", Value = "True" },
                        new() { Name = "Spacing around tags", Value = "True" },
                        new() { Name = "Check for tag id mismatch", Value = "False" },
                        new() { Name = "Ignore formatting tags", Value = "True" },
                        new() { Name = "Ignore locked segments", Value = "False" },
                        new() { Name = "Ignore difference between normal and non-breaking space", Value = "False" }
                    ]
                }
            ];
        }

        public override Dictionary<string, string> SettingIdToUiStringMap { get; set; } = new()
        {
            ["AddedTagsErrorLevel"] = "Tags added",
            ["DeletedTagsErrorLevel"] = "Tags deleted",
            ["TagOrderChangedErrorLevel"] = "Tag order change",
            ["GhostTagErrorLevel"] = "Ghost tags",
            ["SpaceAroundTagsErrorLevel"] = "Spacing around tags",
            ["IgnoreFormattingTags"] = "Ignore formatting tags",
            ["IgnoreLockedSegments"] = "Ignore locked segments",
            ["TreatNonBreakingSpaceAsSpace"] = "Ignore difference between normal and non-breaking space",
            ["CheckForIncludeTagId"] = "Check for tag id mismatch"
        };
    }
}