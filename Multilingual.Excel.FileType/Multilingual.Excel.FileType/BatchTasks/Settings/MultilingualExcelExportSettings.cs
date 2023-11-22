using Sdl.Core.Settings;

namespace Multilingual.Excel.FileType.BatchTasks.Settings
{
	public class MultilingualExcelExportSettings : SettingsGroup
	{
		public override string Id => "MultilingualExcelExportSettings";

		public MultilingualExcelExportSettings ResetToDefaults()
		{
			return this;
		}

		protected override object GetDefaultValue(string settingId)
		{
			return base.GetDefaultValue(settingId);
		}
	}
}
