using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsStoreAction : AbstractTellMeAction
	{
		public CopyTagsStoreAction()
		{
			Name = "Download TradosCopyTags from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/26");
		}

		public override bool IsAvailable => true;
		public override string Category => "TradosCopyTags results";
		public override Icon Icon => PluginResources.Download;
	}
}