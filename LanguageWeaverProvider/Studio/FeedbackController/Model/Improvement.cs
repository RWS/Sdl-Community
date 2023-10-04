using Newtonsoft.Json;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class Improvement
	{
		[JsonProperty("text")]
		public string Text { get; set; }
	}
}