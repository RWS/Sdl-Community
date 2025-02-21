using System.Collections.Generic;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Service.Model
{
	public class TranslationResponse
	{
		public List<TranslationDetails> Translations { get; set; }
	}
}