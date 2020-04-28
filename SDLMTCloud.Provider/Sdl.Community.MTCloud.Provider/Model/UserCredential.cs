using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class UserCredential
	{
		[JsonProperty("username")]
		public string UserName { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }
	}
}
