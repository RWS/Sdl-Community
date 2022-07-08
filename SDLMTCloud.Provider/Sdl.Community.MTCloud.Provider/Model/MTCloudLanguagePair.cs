using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class MTCloudLanguagePair
	{
		[JsonProperty("sourceLanguageId")]
		public string SourceLanguageId { get; set; }

		[JsonProperty("targetLanguageId")]
		public string TargetLanguageId { get; set; }

		[JsonProperty("model")]
		public string Model { get; set; }

		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("active")]
		public string Active { get; set; }
	}
}
