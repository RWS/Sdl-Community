namespace LanguageWeaverProvider.Model.Options.Interface
{
	public interface IOptions
	{
		IProviderOptions ProviderOptions { get; set; }

		ITranslationOptions TranslationOptions { get; set; }
	}
}