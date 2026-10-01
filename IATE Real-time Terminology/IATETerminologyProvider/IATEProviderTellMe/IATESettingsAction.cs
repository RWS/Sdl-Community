using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.TellMe.ProviderApi;
using System.Drawing;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATESettingsAction : AbstractTellMeAction
	{
        private ICacheProvider _cacheProvider;

        public IATESettingsAction(ICacheProvider cacheProvider)
		{
			Name = "IATE Settings";
			_cacheProvider = cacheProvider;
		}

		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var mainWindow = IATEApplication.GetMainWindow(_cacheProvider);
			mainWindow.ShowDialog();
		}
	}
}