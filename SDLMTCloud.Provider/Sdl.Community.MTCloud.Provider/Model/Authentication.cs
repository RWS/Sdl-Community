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

		public AuthenticationType Type { get; set; }

		public string DisplayName { get; set; }

		public long Index { get; set; }
	}
}