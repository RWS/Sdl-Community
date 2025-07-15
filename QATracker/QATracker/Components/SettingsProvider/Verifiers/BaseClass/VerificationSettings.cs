using QATracker.Components.SettingsProvider.Model;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers.BaseClass
{
    public class VerificationSettings : VerificationSettingsTreeNode
    {
        public virtual Dictionary<string, string> SettingIdToUiStringMap { get; set; }

        public VerificationSettingValue this[string settingId]
        {
            get => !SettingIdToUiStringMap.TryGetValue(settingId, out var uiString)
                ? null
                : FindSettingValueRecursive(uiString);
            set
            {
                if (!SettingIdToUiStringMap.TryGetValue(settingId, out var uiString) || value == null)
                    return;

                var target = FindSettingValueRecursive(uiString);
                if (target != null)
                    target.Value = value.Value;
            }
        }

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

                    Children = null;
                    Values = [new() { Name = "Enabled", Value = "False" }];
                    break;
                }

                this[settingsCategory.Key].Value = settingsCategory.Value;
            }
        }

        public VerificationSettingsTreeNode ToSettingsTreeNode() => new()
        {
            Name = Name,
            Children = Children
        };
    }
}