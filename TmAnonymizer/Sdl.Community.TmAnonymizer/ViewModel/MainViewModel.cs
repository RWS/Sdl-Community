namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private TranslationMemoryViewModel _tmViewModel;

		public TranslationMemoryViewModel TmViewModel
		{
			get => _tmViewModel;
			set
			{
				_tmViewModel = value;
				OnPropertyChanged(nameof(TmViewModel));
			}
		}
	
		public TranslationViewModel TranslationViewModel { get; set; }
		public SystemFieldsViewModel SystemFieldsViewModel { get; set; }
		public CustomFieldsViewModel CustomFieldsViewModel { get; set; }
		public MainViewModel()
		{
			_tmViewModel = new TranslationMemoryViewModel();
			TranslationViewModel = new TranslationViewModel(_tmViewModel);
			SystemFieldsViewModel =new SystemFieldsViewModel(_tmViewModel);
			CustomFieldsViewModel=new CustomFieldsViewModel(_tmViewModel);
		}
	}
}
