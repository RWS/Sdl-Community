using Sdl.Core.Settings;
using System.Collections.Generic;
using System.ComponentModel;

namespace QATracker.BatchTasks
{
    public class VerifyFilesExtendedSettings : SettingsGroup
    {
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