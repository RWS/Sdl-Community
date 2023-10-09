using Newtonsoft.Json;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class Translation
	{
		[JsonProperty("sourceLanguageId")]
		public string SourceLanguageId { get; set; }

		[JsonProperty("targetLanguageId")]
		public string TargetLanguageId { get; set; }

		[JsonProperty("model")]
		public string Model { get; set; }

		[JsonProperty("sourceText")]
		public string SourceText { get; set; }

		[JsonProperty("targetMTText")]
		public string TargetMTText { get; set; }

		[JsonProperty("qualityEstimationMT")]
		public string QualityEstimationMT { get; set; }
	}
}