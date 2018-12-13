using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "Advanced Display Filter wiki in the community";
		}

		public override string Category => "Community Advanced Display Filter results";

		public override Icon Icon => PluginResources.Question;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3130.community-advanced-display-filter");
		}
	}
}