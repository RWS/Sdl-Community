using System;
using Sdl.Community.GroupShareKit;

namespace GetTermbaseDetails
{
	class Program
	{
		static void Main(string[] args)
		{
			var token =  GroupShareClient.GetRequestToken(
				"username",
				"pass",
				new Uri( "url"),
				GroupShareClient.AllScopes).Result;

			var gsClient = GroupShareClient.AuthenticateClient(
				token,
				"username",
				"pass",
				new Uri("url"),
				GroupShareClient.AllScopes).Result;
			var termbases = gsClient.Terminology.GetTermbaseById("GermanCharacters").Result;
		}
	}
}
