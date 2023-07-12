using LanguageWeaverProvider.Model.Options.Interface;

namespace LanguageWeaverProvider.ViewModel
{
	public class MainViewModel
	{
		public MainViewModel(ITranslationOptions options)
		{
			TranslationOptions = options;
		}

		public ITranslationOptions TranslationOptions { get; set; }
	}
}