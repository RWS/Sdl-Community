using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FileTypeSupport.MXLIFF.TellMe
{
	public class MXLIFFCommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "MXLIFF results";
		public override Icon Icon => PluginResources.Question;

		public MXLIFFCommunityWikiAction()
		{
			Name = "MXLIFF file type support plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/4036/mxliff-file-type-support");
		}
	}
}