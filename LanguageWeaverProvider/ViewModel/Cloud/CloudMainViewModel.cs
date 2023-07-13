using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudMainViewModel : BaseViewModel, IMainProviderViewModel
	{
		public CloudMainViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
		}

		public ITranslationOptions TranslationOptions { get; set; }
	}
}