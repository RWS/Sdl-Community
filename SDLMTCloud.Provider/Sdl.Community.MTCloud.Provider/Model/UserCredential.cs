using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class UserCredential
	{
		[JsonProperty("password")]
		public string Password { get; set; }

		[JsonProperty("username")]
		public string UserName { get; set; }
	}
}