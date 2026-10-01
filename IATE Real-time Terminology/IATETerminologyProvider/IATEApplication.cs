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

		public static ProjectsController ProjectsController
			=> _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

		public static MainWindow GetMainWindow(
            ICacheProvider cacheProvider, 
            IConnectionProvider connectionProvider, 
            IInventoriesProvider inventoriesProvider)
		{
            if (!IsInitialized)
            {
                Task.Run(async () => await ExecuteAsync(connectionProvider, inventoriesProvider)).GetAwaiter().GetResult();
            }

            var settingsModel = SettingsService.GetSettingsForCurrentProject();
			if (!connectionProvider.EnsureConnection()) return null;

			var listOfViewModels = new List<ISettingsViewModel>
			{
				new DomainsAndTermTypesFilterViewModel(inventoriesProvider),
				new FineGrainedFilterViewModel(inventoriesProvider)
			};

			var mainWindow = new MainWindow(listOfViewModels, settingsModel ?? new SettingsModel(), cacheProvider, new MessageBoxService());

			return mainWindow;
		}

		public static async Task ExecuteAsync(IConnectionProvider connectionProvider, IInventoriesProvider inventoriesProvider)
		{
			Log.Setup();
			Logger.Info("--> IATE Initialize Application");

			try
			{
				Logger.Info("--> Try to login");
				
				var success = connectionProvider.Login("SDL_PLUGIN", "E9KWtWahXs4hvE9z");
				if (success)
				{
					await inventoriesProvider.Initialize();
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