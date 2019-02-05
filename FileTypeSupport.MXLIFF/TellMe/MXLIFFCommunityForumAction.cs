using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FileTypeSupport.MXLIFF.TellMe
{
	public class MXLIFFCommunityForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "MXLIFF results";
		public override Icon Icon => PluginResources.ForumIcon;

		public MXLIFFCommunityForumAction()
		{
			Name = "SDL Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("http://community.sdl.com/appsupport");
		}
	}
}