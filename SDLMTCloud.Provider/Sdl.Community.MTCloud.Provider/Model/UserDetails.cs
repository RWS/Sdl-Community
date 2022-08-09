using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class UserDetails
	{
		[JsonProperty("accountId")]
		public long AccountId { get; set; }

		[JsonProperty("clientId")]
		public string ClientId { get; set; }

		[JsonProperty("userId")]
		public string UserId { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }
	}
}