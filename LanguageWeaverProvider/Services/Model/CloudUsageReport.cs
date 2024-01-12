using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudUsageReport
	{
		[JsonProperty("outputWordCount")]
		public int OutputWordCount { get; set; }

		[JsonProperty("outputCharCount")]
		public int OutputCharCount { get; set; }

		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("inputWordCount")]
		public int InputWordCount { get; set; }

		[JsonProperty("inputCharCount")]
		public int InputCharCount { get; set; }
	}
}