using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "Trados Export Analysis Reports wiki in the RWS Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5254/trados-studio-export-analysis-reports");
		}

		public override bool IsAvailable => true;
		public override string Category => "Trados Export Analysis Reports results";
		public override Icon Icon => PluginResources.Question;
	}
}