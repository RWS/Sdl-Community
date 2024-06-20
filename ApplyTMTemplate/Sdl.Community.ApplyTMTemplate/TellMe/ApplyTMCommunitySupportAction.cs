using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	public class ApplyTMCommunitySupportAction : AbstractTellMeAction
	{
		public ApplyTMCommunitySupportAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}

		public override bool IsAvailable => true;

		public override string Category => "applyTM Template results";

		public override Icon Icon => PluginResources.Question;
	}
}