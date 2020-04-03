using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class MTCloudDictionary
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("source")]
		public string Source { get; set; }

		[JsonProperty("target")]
		public string Target { get; set; }

		[JsonProperty("dictionaryId")]
		public string DictionaryId { get; set; }
	}
}