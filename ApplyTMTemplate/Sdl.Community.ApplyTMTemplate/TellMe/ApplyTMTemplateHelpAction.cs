using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	public class ApplyTMTemplateHelpAction : AbstractTellMeAction
	{
		public ApplyTMTemplateHelpAction()
		{
			Name = "Apply TM Template wiki in the RWS Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/4567/applytm-template");
		}

		public override bool IsAvailable => true;

		public override string Category => "Apply TM Template results";

		public override Icon Icon => PluginResources.Question;
	}
}