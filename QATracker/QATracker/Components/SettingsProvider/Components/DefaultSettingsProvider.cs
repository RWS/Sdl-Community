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
        var verificationSettings = CategoryMap.CreateVerificationSettings(verifier);

        var categories = new List<VerificationSettingsTreeNode>();
        var checkerSettings = new VerificationSettingsTreeNode
        {
            Name = verificationSettings.Name,
            Children = categories
        };

        foreach (var category in verificationSettings.CategoriesList)
        {
            var subcategories = new List<VerificationSettingValue>();
            categories.Add(new VerificationSettingsTreeNode
            {
                Name = category,
                Values = subcategories
            });
            var subcategoryNameList = verificationSettings.SubcategoriesMap[category];
            foreach (var subcategory in subcategoryNameList)
            {
                var sdlprojSettingString =
                    verificationSettings.UiStringToSdlprojStringMap[category][subcategory];
                subcategories.Add(new VerificationSettingValue
                {
                    Name = subcategory,
                    Value = verificationSettings.DefaultSubcategoryValuesMap[sdlprojSettingString].ToString()
                });
            }
        }
        return checkerSettings;
    }

    public static List<string> GetVerifiersList() => DefaultVerifiers.Concat(GetVerifierPlugins()).ToList();

    //TODO: Implement
    private static List<string> GetVerifierPlugins() => [];
}