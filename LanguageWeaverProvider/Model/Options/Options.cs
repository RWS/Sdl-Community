using LanguageWeaverProvider.Model.Options.Interface;

namespace LanguageWeaverProvider.Model.Options
{
	public class Options : IOptions
	{
		public IProviderOptions ProviderOptions { get; set; }

		public ITranslationOptions TranslationOptions { get; set; }
	}
}