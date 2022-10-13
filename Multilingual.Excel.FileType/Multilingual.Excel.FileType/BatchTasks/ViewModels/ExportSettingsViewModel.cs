using Multilingual.Excel.FileType.BatchTasks.Settings;
using Multilingual.Excel.FileType.Models;

namespace Multilingual.Excel.FileType.BatchTasks.ViewModels
{
	public class ExportSettingsViewModel : BaseModel
	{
		public ExportSettingsViewModel(MultilingualExcelExportSettings settings)
		{
			Settings = settings;
		}

		public MultilingualExcelExportSettings Settings { get; }

		public MultilingualExcelExportSettings SaveSettings()
		{
			return Settings;
		}

		public MultilingualExcelExportSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();

			return Settings;
		}
	}
}
