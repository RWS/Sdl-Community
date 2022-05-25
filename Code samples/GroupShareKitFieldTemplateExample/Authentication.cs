using System;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit;

namespace GroupShareKitFieldTemplateExample
{
	public class Authentication
	{
		public async Task<GroupShareClient> Login(string userName, string password, string uri)
		{
			var client = await GroupShareClient.AuthenticateClient(userName,
				password,
				new Uri(uri),
				GroupShareClient.AllScopes);

			return client;
		}
	}
}
