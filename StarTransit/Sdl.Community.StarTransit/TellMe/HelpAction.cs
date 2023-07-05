using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "StarTransit wiki in the RWS Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3270/star-transit-transitpackage-handler");
		}

		public override bool IsAvailable => true;
		public override string Category => "StarTransit results";
		public override Icon Icon => PluginResources.Question;
	}
}