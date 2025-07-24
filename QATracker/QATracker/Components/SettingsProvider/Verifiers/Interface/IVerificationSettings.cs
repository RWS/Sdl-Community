using QATracker.Components.SettingsProvider.Model;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers.Interface
{
    public interface IVerificationSettings
    {
        string Name { get; set; }
        Dictionary<string, string> SettingIdToUiStringMap { get; set; }

        void LoadSettings(Dictionary<string, string> projectVerificationSetting);

        VerificationSettingsTreeNode ToSettingsValue();
    }
}