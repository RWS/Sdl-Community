using Newtonsoft.Json;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class FeedbackRequest
	{
		[JsonProperty("translation")]
		public Translation Translation { get; set; }

		[JsonProperty("improvement")]
		public Improvement Improvement { get; set; }

		[JsonProperty("rating")]
		public Rating Rating { get; set; }

		[JsonProperty("qualityEstimation")]
		public string QualityEstimation { get; set; }
	}
}