using System.Drawing;
using Sdl.Community.NumberVerifier.GUI;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.NumberVerifier.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} Settings";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var settingsWindow = new TellMeSettingsActionWindow();
			settingsWindow.ShowDialog();
		}
	}
}