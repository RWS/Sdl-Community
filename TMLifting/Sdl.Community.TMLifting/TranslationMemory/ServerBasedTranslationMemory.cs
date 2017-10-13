using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit;
using Sdl.Community.GroupShareKit.Clients;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;

namespace Sdl.Community.TMLifting.TranslationMemory
{
	public class ServerBasedTranslationMemory
	{
		//private string _name;
		//private string _description;
		//private List<TranslationMemoryDetails> _tmDetails;

		//public string Name { get; set; }
		//public string Description { get; set; }
		public List<TranslationMemoryDetails> ServerBasedTMDetails { get; set; }
		public ServerBasedTranslationMemory()
		{
			
		}
		public async Task<ServerBasedTranslationMemory> InitializeAsync()
		{
			var token = await GroupShareClient.GetRequestToken("SDLCommunity", "Commun1tyRocks", new Uri("http://gs2017dev.sdl.com"), GroupShareClient.AllScopes);
			var groupShareClient = await GroupShareClient.AuthenticateClient(token, "SDLCommunity", "Commun1tyRocks", new Uri("http://gs2017dev.sdl.com"), GroupShareClient.AllScopes);
			var users = await groupShareClient.User.GetAllUsers(new UsersRequest(1, 2, 7));

			var tmClient = await groupShareClient.TranslationMemories.GetTms();
			//_tmDetails = tmClient.Items;
			this.ServerBasedTMDetails = tmClient.Items;
			return this;
		}

		public static Task<ServerBasedTranslationMemory> CreateAsync()
		{
			var ret = new ServerBasedTranslationMemory();
			return ret.InitializeAsync();
		}
	}
}
