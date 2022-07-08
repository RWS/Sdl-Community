using System;
using Sdl.Community.MTCloud.Provider.Interfaces;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Credential : ICredential
	{
		public Credential()
		{
			Type = Authentication.AuthenticationType.Studio;
			ValidTo = DateTime.MinValue;
		}

		public string AccountId { get; set; }

		public WorkingPortal AccountRegion
		{
			get;
			set;
		}

		public string Name { get; set; }
		public string Password { get; set; }
		public string Token { get; set; }
		public Authentication.AuthenticationType Type { get; set; }
		public DateTime ValidTo { get; set; }
	}
}