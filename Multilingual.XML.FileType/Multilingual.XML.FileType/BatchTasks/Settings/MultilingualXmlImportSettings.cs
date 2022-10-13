using System.Collections.Generic;
using Sdl.Core.Settings;

namespace Multilingual.XML.FileType.BatchTasks.Settings
{
	public class MultilingualXmlImportSettings : SettingsGroup
	{
		private const string OverwriteTranslationsSettingId = "OverwriteTranslations";
		private const string OriginSystemSettingId = "OriginSystem";
		private const string StatusTranslationUpdatedIdSettingId = "StatusTranslationUpdatedId";
		private const string ExcludeFilterIdsSettingId = "ExcludeFilterIds";

		public override string Id => "MultilingualXmlImportSettings";

		public bool OverwriteTranslations
		{
			get => GetSetting<bool>(OverwriteTranslationsSettingId, (bool)GetDefaultValue(OverwriteTranslationsSettingId));
			set => GetSetting<bool>(OverwriteTranslationsSettingId, (bool)GetDefaultValue(OverwriteTranslationsSettingId)).Value = value;
		}

		public string OriginSystem
		{
			get => GetSetting<string>(OriginSystemSettingId, (string)GetDefaultValue(OriginSystemSettingId));
			set => GetSetting<string>(OriginSystemSettingId, (string)GetDefaultValue(OriginSystemSettingId)).Value = value;
		}

		public string StatusTranslationUpdatedId
		{
			get => GetSetting<string>(StatusTranslationUpdatedIdSettingId, (string)GetDefaultValue(StatusTranslationUpdatedIdSettingId));
			set => GetSetting<string>(StatusTranslationUpdatedIdSettingId, (string)GetDefaultValue(StatusTranslationUpdatedIdSettingId)).Value = value;
		}

		public List<string> ExcludeFilterIds
		{
			get => GetSetting<List<string>>(ExcludeFilterIdsSettingId, (List<string>)GetDefaultValue(ExcludeFilterIdsSettingId));
			set => GetSetting<List<string>>(ExcludeFilterIdsSettingId, (List<string>)GetDefaultValue(ExcludeFilterIdsSettingId)).Value = value;
		}

		public MultilingualXmlImportSettings ResetToDefaults()
		{
			OverwriteTranslations = (bool)GetDefaultValue(OverwriteTranslationsSettingId);
			OriginSystem = (string)GetDefaultValue(OriginSystemSettingId);
			StatusTranslationUpdatedId = (string)GetDefaultValue(StatusTranslationUpdatedIdSettingId);
			ExcludeFilterIds = (List<string>)GetDefaultValue(ExcludeFilterIdsSettingId);
			return this;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case OverwriteTranslationsSettingId: return false;
				case OriginSystemSettingId: return "Trados Studio";
				case StatusTranslationUpdatedIdSettingId: return "Draft";
				case ExcludeFilterIdsSettingId: return new List<string>();
			}

			return base.GetDefaultValue(settingId);
		}
	}
}
