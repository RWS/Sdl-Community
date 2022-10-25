using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.TellMe
{
	public class CommunityForumAction : AbstractTellMeAction
	{
		public CommunityForumAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.ForumIcon;

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}
	}
}