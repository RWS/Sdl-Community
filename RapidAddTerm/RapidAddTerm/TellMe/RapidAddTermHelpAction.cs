using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	public class RapidAddTermHelpAction:AbstractTellMeAction
	{
		public RapidAddTermHelpAction()
		{
			Name = "Rapid Add Term plugin wiki";
		}
		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5195/rapid-add-term");
		}
		public override bool IsAvailable => true;
		public override string Category => "Rapid Add Terms results";
		public override Icon Icon => PluginResources.Question;
	}
}
