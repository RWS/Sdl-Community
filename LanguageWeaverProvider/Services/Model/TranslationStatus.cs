using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class TranslationStatus
	{
		[JsonProperty("translationStatus")]
		public string Status { get; set; }

		public string InputFormat { get; set; }

		public string OutputFormat { get; set; }

		public int TranslationProgressPercent { get; set; }

		public TranslationStats TranslationStats { get; set; }

		public List<TranslationLanguagePair> TranslationLanguagePairs { get; set; }
	}
}