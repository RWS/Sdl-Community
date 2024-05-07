using System.Drawing;
using System.Media;
using Sdl.Community.StarTransit.TellMe.View;
using Sdl.Community.StarTransit;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Settings;

		public SettingsAction()
		{
			Name = string.Format("{0} settings", PluginResources.Plugin_Name);
		}

		private static void ShowDialog()
		{
			SystemSounds.Beep.Play();
			new WarningSettingsView("https://appstore.rws.com/Plugin/45?tab=documentation").ShowDialog();
		}

		public override void Execute()
		{
			ShowDialog();
		}
	}
}