using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Trados.Transcreate.TellMe
{
	public class CommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format("{0} results", PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Question;

		public CommunityWikiAction()
		{
			Name = string.Format("{0} plugin wiki", PluginResources.Plugin_Name);
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5842/trados-transcreate");
		}
	}
}
