using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.TellMe.ProviderApi;
using System.Drawing;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATESettingsAction : AbstractTellMeAction
	{
        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;
        private IInventoriesProvider _inventoriesProvider;

        public IATESettingsAction(ICacheProvider cacheProvider, IConnectionProvider connectionProvider, IInventoriesProvider inventoriesProvider)
		{
			Name = "IATE Settings";
			_cacheProvider = cacheProvider;
			_connectionProvider = connectionProvider;
			_inventoriesProvider = inventoriesProvider;
		}

		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var mainWindow = IATEApplication.GetMainWindow(_cacheProvider, _connectionProvider, _inventoriesProvider);
			mainWindow.ShowDialog();
		}
	}
}