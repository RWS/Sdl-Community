using System.Threading.Tasks;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.Tests
{
	public class AuthenticationService
	{
		public async Task Login()
		{
			var credentials = new Credentials
			{
				Password = "",
				UserName = "",
				ServiceUrl = @""
			};
			await Authentication.Login(credentials);
		}
	}
}
