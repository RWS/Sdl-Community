using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FileTypeSupport.MXLIFF.TellMe
{
	public class MXLIFFCommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "MXLIFF results";
		public override Icon Icon => PluginResources.ForumIcon;

		public MXLIFFCommunityWikiAction()
		{
			Name = "SDL Community MXLIFF file type support plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/4036.mxliff-file-type-support");
		}
	}
}