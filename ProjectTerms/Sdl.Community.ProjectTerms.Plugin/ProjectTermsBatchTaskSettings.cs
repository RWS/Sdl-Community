using Sdl.Core.Settings;
using System.Collections.Generic;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsBatchTaskSettings : SettingsGroup
    {
        private int initialTermsOccurrences = 3;
        private int initialTermsLength = 3;

        public List<string> BlackListSettings
        {
            get { return GetSetting<List<string>>(nameof(BlackListSettings)); }
            set { GetSetting<List<string>>(nameof(BlackListSettings)).Value = value; }
        }

        public int TermsOccurrencesSettings
        {
            get { return GetSetting<int>(nameof(TermsOccurrencesSettings)); }
            set { GetSetting<int>(nameof(TermsOccurrencesSettings)).Value = value; }
        }

        public int TermsLengthSettings
        {
            get { return GetSetting<int>(nameof(TermsLengthSettings)); }
            set { GetSetting<int>(nameof(TermsLengthSettings)).Value = value; }
        }

        protected override object GetDefaultValue(string settingId)
        {
            switch(settingId)
            {
                case nameof(TermsOccurrencesSettings): return initialTermsOccurrences;
                case nameof(TermsLengthSettings): return initialTermsLength;
            }

            return base.GetDefaultValue(settingId);
        }

        public void ResetToDefaults()
        {
            BlackListSettings = new List<string>();
            TermsOccurrencesSettings = initialTermsOccurrences;
            TermsLengthSettings = initialTermsLength;
        }
    }
}
