using System.Collections.Generic;
using MicrosoftTranslatorProvider.Service.Model;

namespace MicrosoftTranslatorProvider.Model
{
	public class LanguagesResponse
	{
		public Dictionary<string, TranslationLanguageResponse> Translation { get; set; }
	}
}