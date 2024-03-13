using System.Media;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	public class SettingsAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "settings", "batch task" };
		private static readonly bool _isAvailable = true;
		public SettingsAction() : base($"{PluginResources.Plugin_Name} Settings", PluginResources.TellMeSettings, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

		private static void ShowDialog()
		{
			SystemSounds.Beep.Play();
			new SdlTmAnonymizer.View.TellMe("https://appstore.rws.com/Plugin/39?tab=documentation").ShowDialog();
		}
	}
}