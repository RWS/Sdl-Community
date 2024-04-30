using System.Drawing;
using System.Windows;
using Sdl.Community.DsiViewer.TellMe.WarningWindow;

namespace Sdl.Community.DsiViewer.TellMe
{
	public class SettingsAction : DsiViewerAbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = "DSI Viewer Settings";
		}

		public override Icon Icon => PluginResources.Settings;

		public override void Execute()
		{
			var settingsWarningWindow = new SettingsActionWarning("https://appstore.rws.com/Plugin/25?tab=documentation");
			settingsWarningWindow.Show();
		}
	}
}