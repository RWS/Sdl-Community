using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudTranslationRequest
	{
		[JsonProperty("dictionaries")]
		public object[] Dictionaries { get; set; }

		[JsonProperty("input")]
		public string[] Input { get; set; }

		[JsonProperty("inputFormat")]
		public string InputFormat { get; set; }

		[JsonProperty("model")]
		public string Model { get; set; }

		[JsonProperty("sourceLanguageId")]
		public string SourceLanguageId { get; set; }

		[JsonProperty("targetLanguageId")]
		public string TargetLanguageId { get; set; }

		[JsonProperty("linguisticOptions")]
		public Dictionary<string, string> LinguisticOptions { get; set; }

		[JsonProperty("qualityEstimation")]
		public int QualityEstimation { get; set; }
	}
}