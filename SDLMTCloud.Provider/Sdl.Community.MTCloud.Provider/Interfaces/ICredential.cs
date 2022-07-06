using System;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ICredential
	{
		string AccountId { get; set; }
		WorkingPortal AccountRegion { get; set; }
		string Name { get; set; }
		string Password { get; set; }
		string Token { get; set; }
		Authentication.AuthenticationType Type { get; set; }
		DateTime ValidTo { get; set; }
	}
}