using System;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.IATETerminologyProvider
{
	[ApplicationInitializer]
	public class IATEApplication : IApplicationInitializer
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		
		public static ConnectionProvider ConnectionProvider { get; private set; }

		public static InventoriesProvider InventoriesProvider { get; set; }

		public async void Execute()
		{
			Log.Setup();
			Logger.Info("-->IATE Application Initializer");
			
			try
			{
				Logger.Info("-->Try to get domains");

				ConnectionProvider = new ConnectionProvider();
				var success = ConnectionProvider.Login("SDL_PLUGIN", "E9KWtWahXs4hvE9z");
				if (success)
				{
					InventoriesProvider = new InventoriesProvider(ConnectionProvider);
					await InventoriesProvider.Initialize();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
	}
}
