using System;
using QATracker.Components.SettingsProvider.Components;
using QATracker.Components.SettingsProvider.Model;
using System.Linq;

namespace QATracker.Components.SettingsProvider.Extension
{
    public static class VerificationSettingsTreeNodeExtension
    {
        public static void Set(this VerificationSettingsTreeNode settings, string settingId, string value)
        {
            var settingUi = CategoryMap.SdlprojStringToUiStringMap[settingId];
            if (settingUi is null)
                throw new Exception($"Unknown settingId found in project: {settingId}");

            var category =
                CategoryMap.SubcategoriesMap.FirstOrDefault(settingsList => settingsList.Value.Contains(settingUi)).Key;

            if (category == null) throw new Exception($"Setting not mapped to category {settingId}({settingUi})");

            var verificationSettingsTreeNode = settings.Children.FirstOrDefault(c => c.Name == category);

            if (verificationSettingsTreeNode == null)
                throw new Exception("Incorrect default settings generated!");

            verificationSettingsTreeNode.Values.First(v => v.Name == settingUi).Value = value;
        }
    }
}