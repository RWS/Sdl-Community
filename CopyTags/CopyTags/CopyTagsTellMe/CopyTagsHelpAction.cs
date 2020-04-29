using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsHelpAction : AbstractTellMeAction
	{
		public CopyTagsHelpAction()
		{
			Name = "SDL Copy Tags wiki in the SDL Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5247/sdl-copy-tags");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLCopyTags results";
		public override Icon Icon => PluginResources.Question;
	}
}
