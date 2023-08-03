using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model
{
	public class AccessToken
	{
		[JsonProperty("accessToken")]
		public string Token { get; set; }

		public string TokenType { get; set; }

		public long ValidityInSeconds { get; set; }

		public long ExpiresAt { get; set; }
	}
}