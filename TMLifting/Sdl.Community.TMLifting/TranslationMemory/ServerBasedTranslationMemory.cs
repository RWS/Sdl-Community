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

namespace Sdl.Community.TMLifting.TranslationMemory
{
	public class ServerBasedTranslationMemory
	{
		public List<TranslationMemoryDetails> ServerBasedTMDetails { get; set; }
		public GroupShareClient GroupShareClient { get; set; }

		public ServerBasedTranslationMemory()
		{
			
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

		public void GetUserCredentials()
		{
			var thisAssembly = Assembly.GetExecutingAssembly();
			var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith("Sdl.Community.TMLifting.GSKit.Sdl.TranslationStudio.ProjectManagement.dll"));
			if (resources.Any())
			{
				var resourceName = resources.First();
				using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
				{
					var block = new byte[stream.Length];
					try
					{
						stream.Read(block, 0, block.Length);
						var sdlTranslationStudioProjectManagementAssembly = Assembly.Load(block);
						var projectServersFacadeType = sdlTranslationStudioProjectManagementAssembly.GetType("Sdl.TranslationStudio.ProjectManagement.ProjectServerSettings.Facade.ProjectServersFacade");
						var x = Activator.CreateInstance(projectServersFacadeType);
						var constructor = projectServersFacadeType.GetConstructor(
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
						null, Type.EmptyTypes, null);
						var projectServersFacadeInstance = constructor.Invoke(new object[] { });
						var method = projectServersFacadeType.GetMethod("GetUserCredentials",BindingFlags.NonPublic | BindingFlags.Instance);
						var result = method.Invoke(projectServersFacadeInstance, new object[] { new Uri("http://gs2017dev.sdl.com")});
					}
					catch (IOException)
					{

					}
					catch (BadImageFormatException)
					{

					}
				}
			}
				/////////////////////
			//	var sdlTranslationStudioProjectManagementAssembly = Assembly.LoadFrom(@"C:\Repository\SDL-Community\TMLifting\Sdl.Community.TMLifting\GSKit\Sdl.TranslationStudio.ProjectManagement.dll");
			//var nullServerConnectionServiceType = sdlTranslationStudioProjectManagementAssembly.GetType("Sdl.TranslationStudio.ProjectManagement.ProjectServerSettings.Facade.Nulls.NullServerConnectionService");

			//var nullServerConnectionServiceCtor = nullServerConnectionServiceType.GetConstructor(
			//			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			//			null, Type.EmptyTypes, null);
			//var nullServerConnectionServiceInstance = Activator.CreateInstance(nullServerConnectionServiceType);
			//var getUserCredentialsMethods = nullServerConnectionServiceType.GetMethods();
			//var x = getUserCredentialsMethods[0].Name;
			//var method = nullServerConnectionServiceType.GetMethod("GetUserCredentials");
			//var result = method.Invoke(nullServerConnectionServiceInstance, new object[] { new Uri("http://gs2017dev.sdl.com"), false });

			//var result = method.Invoke(serverConnectionServicInstance, new object[] { new Uri("http://gs2017dev.sdl.com"), false });
			//return Sdl.Desktop.Platform.StudioPlatform.Studio.ActiveWindow.ServiceContext.GetService<Sdl.Desktop.Platform.ServerConnectionPlugin.IServerConnectionService>().GetUserCredentials(new Uri("http://gs2017dev.sdl.com"), false);

		}

		public static I CreateInstance<I>() where I : class
		{
			var serverConnectionPluginAssembly =
				Assembly.LoadFrom(@"C:\Repository\SDL-Community\TMLifting\Sdl.Community.TMLifting\GSKit\Sdl.Desktop.Platform.ServerConnectionPlugin.dll");

			var serverConnectionService = serverConnectionPluginAssembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.IServerConnectionService");
			return Activator.CreateInstance(serverConnectionService) as I;
		}
	}
}
