using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InSource.Tellme
{
	public class InSourceHelpAction : AbstractTellMeAction
	{
		public InSourceHelpAction()
		{
			Name = "Trados InSource! wiki in the RWS Community";
		}
		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3267/trados-insource");
		}

		public override bool IsAvailable => true;
		public override string Category => "Trados InSource! results";
		public override Icon Icon => PluginResources.question;
	}
}
