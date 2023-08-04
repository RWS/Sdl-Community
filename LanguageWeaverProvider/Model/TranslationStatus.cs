using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model
{
	public class TranslationStatus
	{
		public List<TranslationLanguagePair> TranslationLanguagePairs { get; set; }
		[JsonProperty("translationStatus")]
		public string Status { get; set; }
		public string InputFormat { get; set; }
		public string OutputFormat { get; set; }
		public TranslationStats TranslationStats { get; set; }
		public int TranslationProgressPercent { get; set; }
	}
}