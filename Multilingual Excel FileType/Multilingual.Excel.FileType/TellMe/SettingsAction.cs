using System.Drawing;
using Multilingual.Excel.FileType.TellMe.WarningWindow;
using Sdl.TellMe.ProviderApi;

namespace Multilingual.Excel.FileType.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		private SettingsActionWarning _settingsWindow;

		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} Settings";
		}

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public SettingsActionWarning SettingsWindow => _settingsWindow ??=
			new SettingsActionWarning("https://appstore.rws.com/Plugin/17?tab=documentation");

        public override void Execute() => SettingsWindow.ShowDialog();
    }
}