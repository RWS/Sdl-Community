using Sdl.Core.Settings;

namespace Reports.Viewer.Api.Model
{
	public class ReportsViewerSettings : SettingsGroup
	{
		private const string ReportsSettingId = "Reports";
		private const string CustomReportTemplateSettingsId = "CustomReportTemplates";
		private const string UICultureNameSettingId = "UICultureName";

		public Setting<string> ReportsJson
		{
			get => GetSetting<string>(ReportsSettingId);
			set => GetSetting<string>(ReportsSettingId).Value = value;
		}

		public Setting<string> CustomReportTemplatesJson
		{
			get => GetSetting<string>(CustomReportTemplateSettingsId);
			set => GetSetting<string>(CustomReportTemplateSettingsId).Value = value;
		}

		public Setting<string> UICultureName
		{
			get => GetSetting<string>(UICultureNameSettingId);
			set => GetSetting<string>(UICultureNameSettingId).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case ReportsSettingId:
					return string.Empty;
				case CustomReportTemplateSettingsId:
					return string.Empty;
				case UICultureNameSettingId:
					return "en-US";
				default:
					return base.GetDefaultValue(settingId);
			}
		}
	}
}
