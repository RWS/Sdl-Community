using QATracker.Components.SettingsProvider.Model;
using System.Collections.Generic;
using QATracker.Extension;

namespace QATracker.Components.SettingsProvider.Verifiers.BaseClass
{
    public class VerificationSettings : VerificationSettingsTreeNode
    {
        public virtual Dictionary<string, string> SettingIdToUiStringMap { get; set; }

        public VerificationSettingsTreeNode this[string settingId] =>
            !settingId.EndsWithDigits()
                ? FindSettingValueRecursive(settingId)
                : CreateAndReturnNestedValue(settingId);

        //For settings with unlimited number of values
        private VerificationSettingsTreeNode CreateAndReturnNestedValue(string settingId)
        {
            var parentSettingId = settingId.TrimEndingDigits();

            var settingParent = FindSettingValueRecursive(parentSettingId);
            if (settingParent == null)
                return null;

            var settingsTreeNode = new VerificationSettingsTreeNode
            {
                Name = $"{settingParent.Name}{settingId.GetEndingDigits()}",
            };

            settingParent.Values.Add(settingsTreeNode);
            return settingsTreeNode;
        }

        public void LoadSettings(Dictionary<string, string> projectVerificationSettings)
        {
            if (projectVerificationSettings.Keys.Count <= 0)
                return;

            foreach (var settingsCategory in projectVerificationSettings)
            {
                var settingsCategoryKey = settingsCategory.Key;

                if (settingsCategoryKey == "Enabled")
                {
                    if (bool.Parse(settingsCategory.Value))
                        continue;

                    Values = [new() { Name = "Enabled", Value = "False" }];
                    break;
                }

                try
                {
                    this[settingsCategoryKey].Value = settingsCategory.Value;
                }
                catch { }
            }
        }

        public virtual VerificationSettingsTreeNode ToSettingsValue()
        {
            ReplaceNamesWithUiStringsRecursive(Values);
            return new VerificationSettingsTreeNode
            {
                Name = Name,
                Values = Values
            };
        }



        private void ReplaceNamesWithUiStringsRecursive(List<VerificationSettingsTreeNode> nodes)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
            {
                var nodeName = node.Name;

                if (nodeName == "Enabled")
                    continue;

                if (!nodeName.EndsWithDigits())
                {
                    if (SettingIdToUiStringMap.TryGetValue(nodeName, out var uiString))
                        node.Name = uiString;
                }
                else
                {
                    if (SettingIdToUiStringMap.TryGetValue(nodeName.TrimEndingDigits(), out var uiString))
                        node.Name = $"{uiString} {nodeName.GetEndingDigits()}";
                }
                ReplaceNamesWithUiStringsRecursive(node.Values);
            }
        }
    }
}