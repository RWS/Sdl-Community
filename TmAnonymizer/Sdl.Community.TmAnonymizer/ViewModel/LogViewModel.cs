namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class LogViewModel : ViewModelBase
	{
		private readonly TranslationMemoryViewModel _model;

		public LogViewModel(TranslationMemoryViewModel model)
		{
			_model = model;
		}
	}
}