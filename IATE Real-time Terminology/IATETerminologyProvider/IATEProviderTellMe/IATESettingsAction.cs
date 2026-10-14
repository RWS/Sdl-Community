using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.TellMe.ProviderApi;
using System.Drawing;
using TradosStudio.API.ProjectManagement;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATESettingsAction : AbstractTellMeAction
	{
        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;
        private IInventoriesProvider _inventoriesProvider;
        private IProjectsRegistry _projectsRegistry;

        public IATESettingsAction(
            ICacheProvider cacheProvider, 
            IConnectionProvider connectionProvider, 
            IInventoriesProvider inventoriesProvider,
            IProjectsRegistry projectsRegistry)
		{
			Name = "IATE Settings";
			_cacheProvider = cacheProvider;
			_connectionProvider = connectionProvider;
			_inventoriesProvider = inventoriesProvider;
			_projectsRegistry = projectsRegistry;
		}

		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var mainWindow = IATEApplication.GetMainWindow(
                _cacheProvider, 
                _connectionProvider, 
                _inventoriesProvider, 
                _projectsRegistry);

			mainWindow.ShowDialog();
		}
	}
}