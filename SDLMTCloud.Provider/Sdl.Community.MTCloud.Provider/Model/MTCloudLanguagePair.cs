using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class MTCloudLanguagePair
	{
		[JsonProperty("active")]
		public string Active { get; set; }

		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("model")]
		public string Model { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("sourceLanguageId")]
		public string SourceLanguageId { get; set; }

		[JsonProperty("targetLanguageId")]
		public string TargetLanguageId { get; set; }
	}
}