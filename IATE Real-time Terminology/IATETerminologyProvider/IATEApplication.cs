using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.Community.IATETerminologyProvider.ViewModel;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sdl.Community.IATETerminologyProvider
{
	public class IATEApplication
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private static ProjectsController _projectsController;

        public static bool IsInitialized { get; private set; }

		public static ConnectionProvider ConnectionProvider { get; private set; }

		public static InventoriesProvider InventoriesProvider { get; set; }

		public static ProjectsController ProjectsController
			=> _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

		public static MainWindow GetMainWindow(ICacheProvider cacheProvider)
		{
            if (!IsInitialized)
            {
                Task.Run(async () => await ExecuteAsync()).GetAwaiter().GetResult();
            }

            var settingsModel = SettingsService.GetSettingsForCurrentProject();
			if (!ConnectionProvider.EnsureConnection()) return null;

			var listOfViewModels = new List<ISettingsViewModel>
			{
				new DomainsAndTermTypesFilterViewModel(),
				new FineGrainedFilterViewModel()
			};

			var mainWindow = new MainWindow(listOfViewModels, settingsModel ?? new SettingsModel(), cacheProvider, new MessageBoxService());

			return mainWindow;
		}

		public static async Task ExecuteAsync()
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
                    IsInitialized = true;
                }
                else
                {
                    IsInitialized = false;
                    Logger.Error("Login failed. Please check your credentials.");
                }
            }
			catch (Exception ex)
			{
                IsInitialized = false;
                Logger.Error($"{ex.Message}\n{ex.StackTrace}");
			}
		}
	}
}