namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Authentication
	{
		public enum AuthenticationType
		{
			Studio = 0,
			User = 1,
			Client
		}

		public string DisplayName { get; set; }
		public long Index { get; set; }
		public AuthenticationType Type { get; set; }
	}
}