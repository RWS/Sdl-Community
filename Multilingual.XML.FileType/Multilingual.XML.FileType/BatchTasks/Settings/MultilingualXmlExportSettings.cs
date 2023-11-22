using Sdl.Core.Settings;

namespace Multilingual.XML.FileType.BatchTasks.Settings
{
	public class MultilingualXmlExportSettings : SettingsGroup
	{
		private const string MonoLanguageSettingId = "MonoLanguage";

		public override string Id => "MultilingualXmlExportSettings";

		public bool MonoLanguage
		{
			get => GetSetting<bool>(MonoLanguageSettingId, (bool)GetDefaultValue(MonoLanguageSettingId));
			set => GetSetting<bool>(MonoLanguageSettingId, (bool)GetDefaultValue(MonoLanguageSettingId)).Value = value;
		}

		public MultilingualXmlExportSettings ResetToDefaults()
		{
			MonoLanguage = (bool)GetDefaultValue(MonoLanguageSettingId);
			return this;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case MonoLanguageSettingId: return false;
			}

			return base.GetDefaultValue(settingId);
		}
	}
}
