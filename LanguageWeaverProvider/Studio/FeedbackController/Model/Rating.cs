using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class Rating
	{
		[JsonProperty("score")]
		public string Score { get; set; }

		[JsonProperty("comments")]
		public IEnumerable<string> Comments { get; set; }
	}
}