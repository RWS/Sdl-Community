using QATracker.Components.SettingsProvider.Model;
using System;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Components;

public static class DefaultSettingsProvider
{
    

    private static VerificationSettingsTreeNode GetNumberVerifierSettings()
    {
        return null;
    }

   
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

    private static VerificationSettingsTreeNode GetTagVerifierSettings()
    {
        return null;
    }

    private static VerificationSettingsTreeNode GetTermVerifierSettings()
    {
        return null;
    }
}