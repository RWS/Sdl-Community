using System.Collections.Generic;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport.Components.SettingsProvider.Verifiers.Interface
{
    public interface IVerificationSettings
    {
        string Name { get; set; }
        Dictionary<string, string> SettingIdToUiStringMap { get; set; }

        void LoadSettings(Dictionary<string, string> projectVerificationSetting);

        VerificationSettingsTreeNode ToSettingsValue();
    }
}