using Sdl.Core.Settings;

namespace Sdl.Community.Reports.Viewer.Model.ProjectSettings
{	
	public class ReportsViewer : SettingsGroup
	{
		private const string ReportsSettingId = "Reports";

		public Setting<string> ReportsJson
		{
			get => GetSetting<string>(ReportsSettingId);
			set => GetSetting<string>(ReportsSettingId).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case ReportsSettingId:
					return string.Empty;
				default:
					return base.GetDefaultValue(settingId);
			}
		}
	}
}
