using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;

namespace MTEnhancedMicrosoftProvider.Model
{
	public class TranslationOption
	{
		public string Name { get; set; }

		public MTEMicrosoftTranslationOptions.ProviderType ProviderType { get; set; }
	}
}