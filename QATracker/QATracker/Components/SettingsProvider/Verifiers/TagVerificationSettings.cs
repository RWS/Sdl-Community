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
                        new() { Name = "AddedTagsErrorLevel", Value = "True" },
                        new() { Name = "DeletedTagsErrorLevel", Value = "True" },
                        new() { Name = "TagOrderChangedErrorLevel", Value = "True" },
                        new() { Name = "GhostTagErrorLevel", Value = "True" },
                        new() { Name = "SpaceAroundTagsErrorLevel", Value = "True" },
                        new() { Name = "CheckForIncludeTagId", Value = "False" },
                        new() { Name = "IgnoreFormattingTags", Value = "True" },
                        new() { Name = "IgnoreLockedSegments", Value = "False" },
                        new() { Name = "TreatNonBreakingSpaceAsSpace", Value = "False" }
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