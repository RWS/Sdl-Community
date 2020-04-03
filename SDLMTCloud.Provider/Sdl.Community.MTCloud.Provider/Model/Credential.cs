using System;
using Sdl.Community.MTCloud.Provider.Interfaces;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Credential: ICredential
	{
		public Credential()
		{
			Type = Authentication.AuthenticationType.Studio;
			Created = DateTime.MinValue;
		}

		public Authentication.AuthenticationType Type { get; set; }

		public string Name { get; set; }

		public string Password { get; set; }

		public string Token { get; set; }

		public string AccountId { get; set; }

		public DateTime Created { get; set; }
	}
}
