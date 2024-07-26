using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsHelpAction : AbstractTellMeAction
	{
		public CopyTagsHelpAction()
		{
			Name = "Trados Copy Tags wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5247/trados-copy-tags");
		}

		public override bool IsAvailable => true;
		public override string Category => "TradosCopyTags results";
		public override Icon Icon => PluginResources.Question;
	}
}
