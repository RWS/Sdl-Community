using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsHelpAction : AbstractTellMeAction
	{
		public CopyTagsHelpAction()
		{
			Name = "Trados Copy Tags wiki in the RWS Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5247/sdl-copy-tags");
		}

		public override bool IsAvailable => true;
		public override string Category => "TradosCopyTags results";
		public override Icon Icon => PluginResources.Question;
	}
}
