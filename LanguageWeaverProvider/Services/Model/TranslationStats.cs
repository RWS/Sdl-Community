namespace LanguageWeaverProvider.Services.Model
{
	public class TranslationStats
	{
		public int InputWordCount { get; set; }

		public int InputCharCount { get; set; }

		public int InputByteCount { get; set; }

		public int TranslationWordCount { get; set; }

		public int TranslationCharCount { get; set; }

		public int TranslationByteCount { get; set; }
	}
}