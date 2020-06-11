using System.Collections.Generic;
using Sdl.Core.Settings;

namespace Sdl.Community.XLIFF.Manager.Model.ProjectSettings
{
	public class XliffManagerProject:  SettingsGroup
	{		
		private const string ProjectFilesSetting = "ProjectFiles";

		public XliffManagerProject()
		{
			ProjectFiles.Value = new List<XliffManagerProjectFile>();
		}
		
		public Setting<List<XliffManagerProjectFile>> ProjectFiles
		{
			get => GetSetting<List<XliffManagerProjectFile>>(ProjectFilesSetting);
			set => GetSetting<List<XliffManagerProjectFile>>(ProjectFilesSetting).Value = value;
		}		
	}
}
