using Sdl.Core.Settings;

namespace Sdl.Community.Transcreate.Model.ProjectSettings
{
	public class SDLTranscreateProject:  SettingsGroup
	{		
		private const string ProjectFilesSettingId = "ProjectFiles";
		private const string BackProjectsSettingId = "BackProjects";

		public Setting<string> ProjectFilesJson
		{
			get => GetSetting<string>(ProjectFilesSettingId);
			set => GetSetting<string>(ProjectFilesSettingId).Value = value;
		}

		public Setting<string> BackProjectsJson
		{
			get => GetSetting<string>(BackProjectsSettingId);
			set => GetSetting<string>(BackProjectsSettingId).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case ProjectFilesSettingId:
					return string.Empty;
				default:
					return base.GetDefaultValue(settingId);
			}
		}
	}
}
