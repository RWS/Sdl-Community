using System.Drawing;
using Multilingual.XML.FileType.TellMe.WarningWindow;
using Sdl.TellMe.ProviderApi;

namespace Multilingual.XML.FileType.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} Settings";
		}

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var settingsWindow = new SettingsActionWarning("https://appstore.rws.com/Plugin/13?tab=documentation");
			settingsWindow.Show();
		}
	}
}