using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATESettingsAction : AbstractTellMeAction
	{
		public IATESettingsAction()
		{
			Name = "IATE Settings";
		}

		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var mainWindow = IATEApplication.GetMainWindow();
			mainWindow.ShowDialog();
		}
	}
}