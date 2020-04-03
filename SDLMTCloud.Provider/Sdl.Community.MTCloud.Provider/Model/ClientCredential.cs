using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ClientCredential
	{
		[JsonProperty("clientId")]
		public string ClientId { get; set; }


		[JsonProperty("clientSecret")]
		public string ClientSecret { get; set; }
	}
}
