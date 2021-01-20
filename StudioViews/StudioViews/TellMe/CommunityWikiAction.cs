using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StudioViews.TellMe
{
	public class CommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Question;

		public CommunityWikiAction()
		{
			Name = "Studio Views plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5620/studioviews");
		}
	}
}
