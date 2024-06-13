using System.Diagnostics;
using System.Drawing;
using System.Media;
using Sdl.TellMe.ProviderApi;
using SDLCopyTags.CopyTagsTellMe.View;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsSettingsAction : AbstractTellMeAction
	{
		public CopyTagsSettingsAction()
		{
			Name = "Trados Copy Tags Settings";
		}

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            new WarningSettingsView("https://appstore.rws.com/Plugin/26?tab=documentation").ShowDialog();
        }

        public override void Execute()
		{
            ShowDialog();
        }

		public override bool IsAvailable => true;
		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.TellMe_Settings;
	}
}