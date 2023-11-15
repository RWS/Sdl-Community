using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class Rating
	{
		[JsonProperty("comments")]
		public List<string> Comments { get; set; }
	}
}