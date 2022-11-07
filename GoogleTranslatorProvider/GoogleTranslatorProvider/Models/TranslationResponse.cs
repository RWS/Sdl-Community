using System.Collections.Generic;

namespace GoogleTranslatorProvider.Models
{
	public class TranslationResponse
	{
		public List<TranslationDetails> Translations { get; set; }
	}
}