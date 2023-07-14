using System;
using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudMainViewModel : BaseViewModel, IMainProviderViewModel
	{
		public CloudMainViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
		}

		public ITranslationOptions TranslationOptions { get; set; }

		private void InitializeCommands()
		{

		}
	}
}