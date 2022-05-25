using Sdl.Core.Settings;

namespace Sdl.Community.XLIFF.Manager.Model.ProjectSettings
{
	public class XliffManagerProject:  SettingsGroup
	{		
		private const string ProjectFilesSettingId = "ProjectFiles";		
				
		public Setting<string> ProjectFilesJson
		{
			get => GetSetting<string>(ProjectFilesSettingId);
			set => GetSetting<string>(ProjectFilesSettingId).Value = value;
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
