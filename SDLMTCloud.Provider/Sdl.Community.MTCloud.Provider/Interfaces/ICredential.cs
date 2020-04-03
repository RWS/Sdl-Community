using System;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ICredential
	{
		Authentication.AuthenticationType Type { get; set; }

		string Name { get; set; }

		string Password { get; set; }

		string Token { get; set; }

		string AccountId { get; set; }

		DateTime Created { get; set; }
	}
}
