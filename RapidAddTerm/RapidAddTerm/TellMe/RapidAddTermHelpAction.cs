using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	public class RapidAddTermHelpAction:AbstractTellMeAction
	{
		public RapidAddTermHelpAction()
		{
			Name = "Rapid Add Term wiki in the SDL Community";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5195/rapid-add-term");
		}
		public override bool IsAvailable => true;
		public override string Category => "Rapid Add Terms results";
		public override Icon Icon => PluginResources.Question;
	}
}
