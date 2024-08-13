using Sdl.ProjectAutomation.Settings;

namespace Plugin_ModifyAutoSubstitutionSettings
{
    public static class Extensions
    {
        public static void SetSetting(this TranslationMemorySettings translationMemorySettings, string settingId, bool value)
        {
            if (value)
                translationMemorySettings.RemoveSetting(settingId);
            else
            {
                var setting = translationMemorySettings.GetSetting<bool>(settingId);
                setting.Value = false;
            }
        }
    }
}
