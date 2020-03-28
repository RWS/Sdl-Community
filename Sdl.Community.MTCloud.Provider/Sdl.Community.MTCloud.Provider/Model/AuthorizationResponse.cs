using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class AuthorizationResponse
	{
		[JsonProperty("accessToken")]
		public string AccessToken { get; set; }

		[JsonProperty("tokenType")]
		public string TokenType { get; set; }

		[JsonProperty("expiresAt")]
		public long ExpiresAt { get; set; }

		[JsonProperty("validityInSeconds")]
		public long ValidityInSeconds { get; set; }
	}
}
