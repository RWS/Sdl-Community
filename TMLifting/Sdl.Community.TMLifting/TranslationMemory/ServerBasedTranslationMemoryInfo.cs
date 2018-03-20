using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit;
using System.ComponentModel;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;

namespace Sdl.Community.TMLifting.TranslationMemory
{
	static class ServerBasedTranslationMemoryInfo
	{
		public static async Task<ServerBasedTranslationMemoryGSKit> CreateAsync(string userName, string password, string uri)
		{
			var sbTMGSKit = new ServerBasedTranslationMemoryGSKit();
			var token = await GroupShareClient.GetRequestToken(userName, password, new Uri(uri), GroupShareClient.AllScopes);
			var groupShareClient = await GroupShareClient.AuthenticateClient(token, userName, password, new Uri(uri),
				GroupShareClient.AllScopes);

			var tmClient = await groupShareClient.TranslationMemories.GetTms();
			sbTMGSKit.GroupShareClient = groupShareClient;
			sbTMGSKit.ServerBasedTMDetails = new BindingList<TranslationMemoryDetails>(tmClient.Items);
			return sbTMGSKit;
		}
	}
}
