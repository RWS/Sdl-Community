using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Model
{
	public class PairModel
    {
		public string DisplayName { get; set; }

		public string SourceLanguageCode { get; set; }

		public string TargetLanguageCode { get; set; }

		public string Model { get; set; }

		public LanguagePair TradosLanguagePair { get; set; }
    }
}