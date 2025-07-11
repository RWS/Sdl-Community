using QATracker.Components.SettingsProvider.Components;
using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers
{
    public class TagVerificationSettings : IVerificationSettings
    {
        public List<string> CategoriesList { get; set; } = [Constants.Common];

        public Dictionary<string, object> DefaultSubcategoryValuesMap { get; set; } = new()
        {
            ["AddedTagsErrorLevel"] = true,
            ["DeletedTagsErrorLevel"] = true,
            ["TagOrderChangedErrorLevel"] = true,
            ["GhostTagErrorLevel"] = true,
            ["SpaceAroundTagsErrorLevel"] = true,
            ["CheckForIncludeTagId"] = false,
            ["IgnoreFormattingTags"] = true,
            ["IgnoreLockedSegments"] = false,
            ["TreatNonBreakingSpaceAsSpace"] = false
        };

        public string Id { get; set; } = Constants.SettingsTagVerifier;

        public string Name { get; set; } = "Tag Verifier";

        public Dictionary<string, string> SdlprojStringToUiStringMap { get; set; } = new()
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

        public Dictionary<string, List<string>> SubcategoriesMap { get; set; } = new()
        {
            ["Common"] =
            [
                "Tags added",
                "Tags deleted",
                "Tag order change",
                "Ghost tags",
                "Spacing around tags",
                "Check for tag id mismatch",
                "Ignore formatting tags",
                "Ignore locked segments",
                "Ignore difference between normal and non-breaking space"
            ]
        };

        public Dictionary<string, Dictionary<string, string>> UiStringToSdlprojStringMap { get; set; } = new()
        {
            [Constants.Common] = new()
            {
                ["Tags added"] = "AddedTagsErrorLevel",
                ["Tags deleted"] = "DeletedTagsErrorLevel",
                ["Tag order change"] = "TagOrderChangedErrorLevel",
                ["Ghost tags"] = "GhostTagErrorLevel",
                ["Spacing around tags"] = "SpaceAroundTagsErrorLevel",
                ["Ignore formatting tags"] = "IgnoreFormattingTags",
                ["Ignore locked segments"] = "IgnoreLockedSegments",
                ["Ignore difference between normal and non-breaking space"] = "TreatNonBreakingSpaceAsSpace",
                ["Check for tag id mismatch"] = "CheckForIncludeTagId"
            }
        };
    }
}