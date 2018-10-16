using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.Studio;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private TranslationMemoryViewModel _tmViewModel;

		public MainViewModel(SettingsService settingsService, TmAnonymizerViewController controller)
		{
			_tmViewModel = new TranslationMemoryViewModel(settingsService, controller);
			
			var excelImportExportService = new ExcelImportExportService();
			var systemFieldsService = new SystemFieldsService(_tmViewModel.TmService);
			var customFieldsService = new CustomFieldsService(_tmViewModel.TmService);

			TranslationViewModel = new ContentFilteringRulesViewModel(_tmViewModel, excelImportExportService);			
			SystemFieldsViewModel = new SystemFieldsViewModel(_tmViewModel, systemFieldsService, excelImportExportService);
			CustomFieldsViewModel = new CustomFieldsViewModel(_tmViewModel, customFieldsService, excelImportExportService);			
		}

		public TranslationMemoryViewModel TmViewModel
		{
			get => _tmViewModel;
			set
			{
				_tmViewModel = value;

				OnPropertyChanged(nameof(TmViewModel));
			}
		}

		public ContentFilteringRulesViewModel TranslationViewModel { get; set; }

		public SystemFieldsViewModel SystemFieldsViewModel { get; set; }

		public CustomFieldsViewModel CustomFieldsViewModel { get; set; }
	}
}
