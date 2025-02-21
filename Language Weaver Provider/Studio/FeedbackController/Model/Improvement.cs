using Newtonsoft.Json;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class Improvement
	{
		public Improvement(string text)
		{
			Text = text;
		}

		[JsonProperty("text")]
		public string Text { get; set; }
	}
}