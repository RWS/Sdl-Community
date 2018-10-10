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
			
			TranslationViewModel = new ContentFilteringRulesViewModel(_tmViewModel);			
			SystemFieldsViewModel = new SystemFieldsViewModel(_tmViewModel, new SystemFieldsService(), new UsersService());
			CustomFieldsViewModel = new CustomFieldsViewModel(_tmViewModel, new CustomFieldsService(), new ExcelImportExportService());			
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
