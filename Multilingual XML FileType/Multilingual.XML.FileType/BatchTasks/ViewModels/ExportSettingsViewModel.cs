using Multilingual.XML.FileType.BatchTasks.Settings;
using Multilingual.XML.FileType.Models;

namespace Multilingual.XML.FileType.BatchTasks.ViewModels
{
	public class ExportSettingsViewModel : BaseModel
	{
		private bool _monoLanguage;

		public ExportSettingsViewModel(MultilingualXmlExportSettings settings)
		{
			Settings = settings;
			MonoLanguage = Settings.MonoLanguage;
		}

		public MultilingualXmlExportSettings Settings { get; }

		public bool MonoLanguage
		{
			get => _monoLanguage;
			set
			{
				if (_monoLanguage == value)
				{
					return;
				}

				_monoLanguage = value;

				OnPropertyChanged(nameof(MonoLanguage));
			}
		}

		public MultilingualXmlExportSettings SaveSettings()
		{
			Settings.MonoLanguage = MonoLanguage;
			return Settings;
		}

		public MultilingualXmlExportSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();
			MonoLanguage = Settings.MonoLanguage;

			return Settings;
		}
	}
}
