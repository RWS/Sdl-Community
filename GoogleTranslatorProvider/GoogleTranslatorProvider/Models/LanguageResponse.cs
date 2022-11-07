using System.Collections.Generic;

namespace GoogleTranslatorProvider.Models
{
	public class LanguageResponse
	{
		public Dictionary<string, LanguageDetails> Translation { get; set; }
	}
}
