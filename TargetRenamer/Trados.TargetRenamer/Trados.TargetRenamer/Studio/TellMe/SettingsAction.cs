using System.Drawing;
using System.Media;
using Trados.TargetRenamer.Studio.TellMe.View;

namespace Trados.TargetRenamer.Studio.TellMe
{
	public class SettingsAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = ["settings"];
		private static readonly string _actionName = Constants.TellMe_Settings_Name;
		private static readonly Icon _icon = PluginResources.TellMe_Settings;
		private static readonly bool _isAvailable = true;

		public SettingsAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

		private static void ShowDialog()
		{
			SystemSounds.Beep.Play();
			_ = new SettingsWarningView(Constants.TellMe_Documentation_Url).ShowDialog();
		}
	}
}