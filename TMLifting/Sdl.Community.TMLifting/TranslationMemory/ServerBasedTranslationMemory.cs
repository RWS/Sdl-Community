using Sdl.Community.TMLifting.Processor;
using Sdl.Community.Toolkit.Core.Services;
//using Sdl.Desktop.Platform;
//using Sdl.Desktop.Platform.ServerConnectionPlugin;
//using Sdl.Enterprise2.Platform.Client.IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit;
using Sdl.Community.GroupShareKit.Clients;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using Sdl.Community.TMLifting.Helpers;

namespace Sdl.Community.TMLifting.TranslationMemory
{
	public class ServerBasedTranslationMemory
	{
		private readonly Assembly _sdlTranslationStudioProjectManagementAssembly;

		public List<TranslationMemoryDetails> ServerBasedTMDetails { get; set; }
		public GroupShareClient GroupShareClient { get; set; }

		public ServerBasedTranslationMemory()
		{
			var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(s => s.EndsWith("Sdl.Community.TMLifting.GSKit.Sdl.TranslationStudio.ProjectManagement.dll"));
			if (resources.Any())
			{
				var resourceName = resources.First();
				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{
					var block = new byte[stream.Length];
					try
					{
						stream.Read(block, 0, block.Length);

						_sdlTranslationStudioProjectManagementAssembly = Assembly.Load(block);
					}
					catch (IOException)
					{

					}
					catch (BadImageFormatException)
					{

					}
				}
			}
		}
		public async Task<ServerBasedTranslationMemory> InitializeAsync(string userName, string password, string uri)
		{
			var token = await GroupShareClient.GetRequestToken(userName, password, new Uri(uri), GroupShareClient.AllScopes);
			var groupShareClient = await GroupShareClient.AuthenticateClient(token, userName, password, new Uri(uri),
				GroupShareClient.AllScopes);

			var tmClient = await groupShareClient.TranslationMemories.GetTms();
			this.GroupShareClient = groupShareClient;
			this.ServerBasedTMDetails = tmClient.Items;
			return this;
		}

		public static Task<ServerBasedTranslationMemory> CreateAsync(string userName, string password, string uri)
		{
			var ret = new ServerBasedTranslationMemory();
			return ret.InitializeAsync(userName, password, uri );
		}

		public async Task<IEnumerable<Uri>> GetServers()
		{
			//http://gs2017dev.sdl.com

			var projectServersFacadeType = GetProjectServersFacadeType();
			var constructor = projectServersFacadeType.GetConstructor(
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			null, Type.EmptyTypes, null);
			var projectServersFacadeInstance = constructor.Invoke(new object[] { });
			var getAllServersMethod = projectServersFacadeType.GetMethod("GetAllServers", BindingFlags.Public | BindingFlags.Instance);
			var servers = await (dynamic) getAllServersMethod.Invoke(projectServersFacadeInstance, new object[] { });

			return servers;
		}

		public UserCredentials GetUserCredentials(Uri uri)
		{
			var projectServersFacadeType = GetProjectServersFacadeType();
			var constructor = projectServersFacadeType.GetConstructor(
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			null, Type.EmptyTypes, null);
			var projectServersFacadeInstance = constructor.Invoke(new object[] { });
			var getUserCredentialsMethod = projectServersFacadeType.GetMethod("GetUserCredentials",BindingFlags.NonPublic | BindingFlags.Instance);
			dynamic credentials = getUserCredentialsMethod.Invoke(projectServersFacadeInstance, new object[] { uri });
			if (credentials != null)
			{
				var userCredentials = new UserCredentials()
				{
					Password = credentials.Password,
					UserName = credentials.UserName
				};
				return userCredentials;
			}
			else
			{
				return new UserCredentials();
			}			
		}

		private Type GetProjectServersFacadeType()
		{
			return _sdlTranslationStudioProjectManagementAssembly.GetType("Sdl.TranslationStudio.ProjectManagement.ProjectServerSettings.Facade.ProjectServersFacade");
		}
	}
}
