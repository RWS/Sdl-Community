using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Term
	{
		[JsonProperty("source")]
		public string Source { get; set; }

		[JsonProperty("target")]
		public string Target { get; set; }
	}
}