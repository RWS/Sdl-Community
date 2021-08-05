using System;
using System.Collections.Generic;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.Community.IATETerminologyProvider.ViewModel;
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

		public static ICacheProvider CacheProvider { get; set; } =
			new CacheProvider(new SqliteDatabaseProvider(new PathInfo()));

		public static MainWindow MainWindow { get; set; }

		public async void Execute()
		{
			Log.Setup();
			Logger.Info("--> IATE Initialize Application");
			
			try
			{
				Logger.Info("--> Try to login");

				ConnectionProvider = new ConnectionProvider();
				var success = ConnectionProvider.Login("SDL_PLUGIN", "E9KWtWahXs4hvE9z");
				if (success)
				{
					InventoriesProvider = new InventoriesProvider(ConnectionProvider);
					await InventoriesProvider.Initialize();
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"{ex.Message}\n{ex.StackTrace}");
			}
		}

		public static MainWindow GetMainWindow(SettingsModel settingsModel = null)
		{
			if (!ConnectionProvider.EnsureConnection()) return null;

			var listOfViewModels = new List<SettingsViewModelBase>
			{
				new DomainsAndTermTypesFilterViewModel(InventoriesProvider, CacheProvider, new MessageBoxService()),
				new FineGrainedFilterViewModel()
			};

			MainWindow = new MainWindow(listOfViewModels, settingsModel);

			return MainWindow;
		}
	}
}
