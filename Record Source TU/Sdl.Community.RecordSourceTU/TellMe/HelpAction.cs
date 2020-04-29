using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RecordSourceTU.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "Record Source TU wiki in the SDL Community";
		}

		public override bool IsAvailable => true;

		public override string Category => "Record Source TU results";

		public override Icon Icon => PluginResources.Question;

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/4721/new-in-version-1-2-0-3");
		}
	}
}