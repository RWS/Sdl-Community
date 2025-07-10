using QATracker.Components.SettingsProvider.Model;
using System.Collections.Generic;
using System.Linq;

namespace QATracker.Components.SettingsProvider.Components;

public static class DefaultSettingsProvider
{
    private static List<string> DefaultVerifiers { get; } =
    [
        Constants.QaVerificationSettings,
        Constants.SettingsTagVerifier,
        Constants.SettingsTermVerifier
    ];

    public static VerificationSettingsTreeNode GetDefaultSettingsForVerifier(string verifier)
    {
        var categories = new List<VerificationSettingsTreeNode>();
        var checkerSettings = new VerificationSettingsTreeNode
        {
            Name = CategoryMap.VerifierNamesMap[verifier],
            Children = categories
        };

        foreach (var category in CategoryMap.VerifierCategoriesMap[verifier])
        {
            var subcategories = new List<VerificationSettingValue>();
            categories.Add(new VerificationSettingsTreeNode
            {
                Name = category,
                Values = subcategories
            });
            var subcategoryNameList = CategoryMap.SubcategoriesMap[category];
            foreach (var subcategory in subcategoryNameList)
            {
                var sdlprojSettingString =
                    CategoryMap.UiStringToSdlprojStringMap[category][subcategory];
                subcategories.Add(new VerificationSettingValue
                {
                    Name = subcategory,
                    Value = CategoryMap.DefaultSubcategoryValuesMap[sdlprojSettingString].ToString()
                });
            }
        }
        return checkerSettings;
    }

    public static List<string> GetVerifiersList() => DefaultVerifiers.Concat(GetVerifierPlugins()).ToList();

    //TODO: Implement
    private static List<string> GetVerifierPlugins() => [];
}