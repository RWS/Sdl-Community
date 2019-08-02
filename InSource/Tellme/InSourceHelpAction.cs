using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InSource.Tellme
{
	public class InSourceHelpAction : AbstractTellMeAction
	{
		public InSourceHelpAction()
		{
			Name = "InSource wiki in the SDL Community";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3267/insource");
		}

		public override bool IsAvailable => true;
		public override string Category => "InSource results";
		public override Icon Icon => PluginResources.question;
	}
}
