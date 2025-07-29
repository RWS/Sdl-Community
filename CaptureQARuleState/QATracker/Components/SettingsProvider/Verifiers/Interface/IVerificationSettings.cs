using System.Collections.Generic;
using CaptureQARuleState.Components.SettingsProvider.Model;

namespace CaptureQARuleState.Components.SettingsProvider.Verifiers.Interface
{
    public interface IVerificationSettings
    {
        string Name { get; set; }
        Dictionary<string, string> SettingIdToUiStringMap { get; set; }

        void LoadSettings(Dictionary<string, string> projectVerificationSetting);

        VerificationSettingsTreeNode ToSettingsValue();
    }
}