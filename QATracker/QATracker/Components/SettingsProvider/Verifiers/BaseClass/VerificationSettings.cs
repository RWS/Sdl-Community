using QATracker.Components.SettingsProvider.Model;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers.BaseClass
{
    public class VerificationSettings : VerificationSettingsTreeNode
    {
        public virtual Dictionary<string, string> SettingIdToUiStringMap { get; set; }

        public VerificationSettingsTreeNode this[string settingId] => FindSettingValueRecursive(settingId);

        public void LoadSettings(Dictionary<string, string> projectVerificationSettings)
        {
            if (projectVerificationSettings.Keys.Count <= 0)
                return;

            foreach (var settingsCategory in projectVerificationSettings)
            {
                if (settingsCategory.Key == "Enabled")
                {
                    if (bool.Parse(settingsCategory.Value))
                        continue;

                    Values = [new() { Name = "Enabled", Value = "False" }];
                    break;
                }

                var verificationSettingValue = this[settingsCategory.Key];

                if (verificationSettingValue.Values is null || verificationSettingValue.Values.Count == 0)
                    verificationSettingValue.Value = settingsCategory.Value;
                else
                    verificationSettingValue.Enabled = settingsCategory.Value;
            }
        }

        private void ReplaceNamesWithUiStringsRecursive(List<VerificationSettingsTreeNode> nodes)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
            {
                if (node.Name == "Enabled")
                    continue;

                if (SettingIdToUiStringMap.TryGetValue(node.Name, out var uiString))
                    node.Name = uiString;

                ReplaceNamesWithUiStringsRecursive(node.Values);
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
    }
}