using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class LinguisticOptions
	{
		[JsonIgnore]
		public string ModelName { get; set; } = string.Empty;

		[JsonProperty("id")]
		public string Id { get; set; } = string.Empty;

		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("systemDefault")]
		public string SystemDefault { get; set; } = string.Empty;

		[JsonProperty("values")]
		public IList<string> Values { get; set; } = new List<string>();
	}
}