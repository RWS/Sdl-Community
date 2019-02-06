namespace Sdl.Community.FailSafeTask
{
    using Sdl.Core.Settings;

    public class FailSafeTaskSettings : SettingsGroup
    {
        public bool CopyToTarget
        {
            get { return GetSetting<bool>(nameof(CopyToTarget)); }
            set { GetSetting<bool>(nameof(CopyToTarget)).Value = value; }
        }

        public bool DeleteGeneratedFiles
        {
            get { return GetSetting<bool>(nameof(DeleteGeneratedFiles)); }
            set { GetSetting<bool>(nameof(DeleteGeneratedFiles)).Value = value; }
        }

        public bool PseudoTranslate
        {
            get { return GetSetting<bool>(nameof(PseudoTranslate)); }
            set { GetSetting<bool>(nameof(PseudoTranslate)).Value = value; }
        }

        protected override object GetDefaultValue(string settingId)
        {
            switch (settingId)
            {
                case nameof(CopyToTarget):
                    return true;

                case nameof(DeleteGeneratedFiles):
                    return true;

                case nameof(PseudoTranslate):
                    return false;

                default:
                    break;
            }

            return base.GetDefaultValue(settingId);
        }
    }
}