using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.TellMe.ProviderApi;
using System.Drawing;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATESettingsAction : AbstractTellMeAction
	{
        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;

        public IATESettingsAction(ICacheProvider cacheProvider, IConnectionProvider connectionProvider)
		{
			Name = "IATE Settings";
			_cacheProvider = cacheProvider;
			_connectionProvider = connectionProvider;
		}

		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var mainWindow = IATEApplication.GetMainWindow(_cacheProvider, _connectionProvider);
			mainWindow.ShowDialog();
		}
	}
}