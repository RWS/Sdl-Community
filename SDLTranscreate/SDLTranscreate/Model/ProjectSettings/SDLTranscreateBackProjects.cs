using Sdl.Core.Settings;

namespace Trados.Transcreate.Model.ProjectSettings
{
	public class SDLTranscreateBackProjects: SettingsGroup
	{
		private const string BackProjectsSettingId = "BackProjects";

		public Setting<string> BackProjectsJson
		{
			get => GetSetting<string>(BackProjectsSettingId);
			set => GetSetting<string>(BackProjectsSettingId).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case BackProjectsSettingId:
					return string.Empty;
				default:
					return base.GetDefaultValue(settingId);
			}
		}
	}
}
