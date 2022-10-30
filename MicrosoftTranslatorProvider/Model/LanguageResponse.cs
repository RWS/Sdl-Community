using System.Collections.Generic;

namespace MicrosoftTranslatorProvider.Model
{
	public class LanguageResponse
	{
		public Dictionary<string, LanguageDetails> Translation { get; set; }
	}
}