using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsSettingsAction : AbstractTellMeAction
	{
		public CopyTagsSettingsAction()
		{
			Name = "Trados CopyTags Settings";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/26");
		}

		public override bool IsAvailable => true;
		public override string Category => "TradosCopyTags results";
		public override Icon Icon => PluginResources.TellMe_Settings;
	}
}