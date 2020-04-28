using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "StarTransit wiki in the SDL Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3270/star-transit");
		}

		public override bool IsAvailable => true;
		public override string Category => "ExportAnalysisReports results";
		public override Icon Icon => PluginResources.Question;
	}
}