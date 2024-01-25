using System.Drawing;
using System.Windows;

namespace Sdl.Community.DsiViewer.TellMe
{
	public class SettingsAction : DsiViewerAbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = "DSI Viewer Settings";
		}

		public override Icon Icon => default;

		public override void Execute()
		{
			MessageBox.Show("See documentation for guidance.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}