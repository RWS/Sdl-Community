using System.Collections.ObjectModel;
using System.Windows.Input;
using Sdl.Desktop.IntegrationApi;
using Sdl.Community.TQA.Commands;
using Sdl.Community.TQA.Model;
using Sdl.Community.TQA.Providers;
using Sdl.Core.Settings;


namespace Sdl.Community.TQA.BatchTask.ViewModel
{
	public class TQAReportingSettingsViewModel : BaseModel, ISettingsAware<TQAReportingSettings>
	{
		private ICommand resetToDefault;
		//private ICommand selectReportingFolder;
		//private string _reportOutputLocation;

		private string _tqaProfileName;
		
		private string _selectedTqaQualityItem;
		private ObservableCollection<string> _tqaQualityItems;

		private readonly ReportProvider _reportProvider;
		private readonly QualitiesProvider _qualitiesProvider;
		private readonly CategoriesProvider _categoriesProvider;

		public TQAReportingSettingsViewModel(ISettingsBundle projectSettings, TQAReportingSettings settings, ReportProvider reportProvider,
			CategoriesProvider categoriesProvider, QualitiesProvider qualitiesProvider)
		{
			Settings = settings;
			_reportProvider = reportProvider;
			_categoriesProvider = categoriesProvider;
			_qualitiesProvider = qualitiesProvider;

			var tqaCategories = categoriesProvider.GetAssessmentCategories(projectSettings);
			var tqaProfileType = categoriesProvider.GetTQAProfileType(tqaCategories);

			var qualities = _qualitiesProvider.GetQualities(tqaProfileType);

			TQAQualityItems = new ObservableCollection<string>(qualities);
			if (TQAQualityItems.Count > 0)
			{
				SelectedTQAQualityItem = settings.TQAReportingQuality;
			}

			TQAProfileName = _reportProvider.GetProfileTypeName(tqaProfileType);
		}

		public TQAReportingSettings Settings { get; set; }

		public string ReportQuality
		{
			get => Settings.TQAReportingQuality;
			set
			{
				if (Settings.TQAReportingQuality == value)
				{
					return;
				}
				Settings.TQAReportingQuality = value;
				OnPropertyChanged(nameof(ReportQuality));
			}
		}

		public ObservableCollection<string> TQAQualityItems
		{
			get => _tqaQualityItems;
			set
			{
				_tqaQualityItems = value;
				OnPropertyChanged(nameof(TQAQualityItems));
			}
		}

		public ICommand ResetToDefault => resetToDefault ?? (resetToDefault = new CommandHandler(ResetSettingsToDefault));

		//public ICommand SelectReportOutputFolder => selectReportingFolder ?? (selectReportingFolder = new CommandHandler(SelectFolder));

		//public string ReportOutputLocation
		//{
		//	get => _reportOutputLocation;
		//	set
		//	{
		//		if (value == _reportOutputLocation)
		//		{
		//			return;
		//		}

		//		_reportOutputLocation = value;
		//		OnPropertyChanged(nameof(ReportOutputLocation));
		//	}
		//}

		public string TQAProfileName
		{
			get => _tqaProfileName;
			set
			{
				if (value == _tqaProfileName)
				{
					return;
				}

				_tqaProfileName = value;
				OnPropertyChanged(nameof(TQAProfileName));
			}
		}

		public string SelectedTQAQualityItem
		{
			get => _selectedTqaQualityItem;
			set
			{
				if (_selectedTqaQualityItem == value)
				{
					return;
				}

				_selectedTqaQualityItem = value;
				OnPropertyChanged(nameof(SelectedTQAQualityItem));

				Settings.TQAReportingQuality = value;
			}
		}

		private void ResetSettingsToDefault(object obj)
		{
			ResetToDefaults();
		}

		public TQAReportingSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();
			Settings.TQAReportingQuality = SelectedTQAQualityItem;

			return Settings;
		}

		public TQAReportingSettings SaveSettings()
		{
			Settings.TQAReportingQuality = SelectedTQAQualityItem;

			return Settings;
		}

		//public void SelectFolder(object obj)
		//{
		//	var folderDialog = new FolderSelectDialog
		//	{
		//		Title = "Select the report folder",
		//		InitialDirectory = ReportOutputLocation
		//	};

		//	if (folderDialog.ShowDialog())
		//	{
		//		ReportOutputLocation = folderDialog.FileName;
		//	}
		//}
	}
}