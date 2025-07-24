using QATracker.Components.SettingsProvider.Model;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers.BaseClass
{
    public class VerificationSettings : VerificationSettingsTreeNode
    {
        public virtual Dictionary<string, string> SettingIdToUiStringMap { get; set; }

        private VerificationSettingsTreeNode this[string settingId]
        {
            get
            {
                var x = !SettingIdToUiStringMap.TryGetValue(settingId, out var uiString)
                    ? null
                    : FindSettingValueRecursive(uiString);

                return x;
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

        public VerificationSettingsTreeNode ToSettingsValue() => new()
        {
            Name = Name,
            Values = Values
        };
    }
}