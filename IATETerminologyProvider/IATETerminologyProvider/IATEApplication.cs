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
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.IATETerminologyProvider
{
	[ApplicationInitializer]
	public class IATEApplication : IApplicationInitializer
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private static ProjectsController _projectsController;

		public static ICacheProvider CacheProvider { get; set; } =
			new CacheProvider(new SqliteDatabaseProvider(new PathInfo()));

		public static ConnectionProvider ConnectionProvider { get; private set; }

		public static InventoriesProvider InventoriesProvider { get; set; }

		public static MainWindow MainWindow { get; set; }

		public static IMessageBoxService MessageBoxService { get; set; } = new MessageBoxService();
		public static IEUProvider EUProvider { get; set; } = new EUProvider();

		public static ProjectsController ProjectsController
			=> _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

		public static MainWindow GetMainWindow()
		{
			var settingsModel = SettingsService.GetSettingsForCurrentProject();
			if (!ConnectionProvider.EnsureConnection()) return null;

			var listOfViewModels = new List<ISettingsViewModel>
			{
				new DomainsAndTermTypesFilterViewModel(),
				new FineGrainedFilterViewModel()
			};

			MainWindow = new MainWindow(listOfViewModels, settingsModel ?? new SettingsModel());

			return MainWindow;
		}

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
	}
}