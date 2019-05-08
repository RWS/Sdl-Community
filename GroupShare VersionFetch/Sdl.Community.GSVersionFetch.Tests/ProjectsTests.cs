using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.GSVersionFetch.Service;
using Xunit;

namespace Sdl.Community.GSVersionFetch.Tests
{
	public class ProjectsTests
	{
		private readonly AuthenticationService _authentication = new AuthenticationService();

		[Fact]
		public async Task GetProjects()
		{
			await _authentication.Login().ConfigureAwait(true);
			var token = Authentication.Token;
		}
	}
}
