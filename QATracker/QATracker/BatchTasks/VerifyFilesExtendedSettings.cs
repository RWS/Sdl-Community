using Sdl.Core.Settings;
using System.Collections.Generic;

namespace QATracker.BatchTasks
{
    public class VerifyFilesExtendedSettings : SettingsGroup
    {
        public bool IncludeIgnoredMessages
        {
            get => GetSetting<bool>(nameof(IncludeIgnoredMessages));
            set => GetSetting<bool>(nameof(IncludeIgnoredMessages)).Value = value;
        }

        public bool IncludeVerificationDetails
        {
            get => GetSetting<bool>(nameof(IncludeVerificationDetails));
            set => GetSetting<bool>(nameof(IncludeVerificationDetails)).Value = value;
        }

        public List<string> ReportStatuses
        {
            get => GetSetting<List<string>>(nameof(ReportStatuses));
            set => GetSetting<List<string>>(nameof(ReportStatuses)).Value = value;
        }

        protected override object GetDefaultValue(string settingId)
        {
            return settingId switch
            {
                nameof(ReportStatuses) => new List<string>(),
                _ => base.GetDefaultValue(settingId)
            };
        }
    }
}