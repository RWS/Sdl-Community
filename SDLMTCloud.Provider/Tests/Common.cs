using System;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Tests
{
	public class Common
	{
		public ICredential GetDefaultCredential(Authentication.AuthenticationType type)
		{
			var credential = new Credential
			{
				Type = type,
				ValidTo = DateTime.UtcNow.AddHours(1),
				Token = "abc123",
				Name = "Name",
				Password = "Password",
				AccountId = "0123456789"
			};

			return credential;
		}
	}
}
