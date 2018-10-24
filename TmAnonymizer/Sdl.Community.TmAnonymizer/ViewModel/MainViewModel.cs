using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.Studio;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private TranslationMemoryViewModel _translationMemoryViewModel;

		public MainViewModel(SettingsService settingsService, SDLTMAnonymizerView controller)
		{
			var contentParsingService = new ContentParsingService();
			_translationMemoryViewModel = new TranslationMemoryViewModel(settingsService, contentParsingService, controller);

			var excelImportExportService = new ExcelImportExportService();
			var serializerService = new SerializerService();
			var systemFieldsService = new SystemFieldsService(_translationMemoryViewModel.TmService, settingsService);
			var customFieldsService = new CustomFieldsService(_translationMemoryViewModel.TmService, settingsService);

			ContentFilteringRulesViewModel = new ContentFilteringRulesViewModel(_translationMemoryViewModel, excelImportExportService);
			SystemFieldsViewModel = new SystemFieldsViewModel(_translationMemoryViewModel, systemFieldsService, excelImportExportService, serializerService);
			CustomFieldsViewModel = new CustomFieldsViewModel(_translationMemoryViewModel, customFieldsService, excelImportExportService, serializerService);
			
			LogViewModel = new LogViewModel(_translationMemoryViewModel, serializerService);
		}

		public TranslationMemoryViewModel TranslationMemoryViewModel
		{
			get => _translationMemoryViewModel;
			set
			{
				_translationMemoryViewModel = value;

				OnPropertyChanged(nameof(TranslationMemoryViewModel));
			}
		}

		public ContentFilteringRulesViewModel ContentFilteringRulesViewModel { get; set; }

		public SystemFieldsViewModel SystemFieldsViewModel { get; set; }

		public CustomFieldsViewModel CustomFieldsViewModel { get; set; }

		public LogViewModel LogViewModel { get; set; }
	}
}
