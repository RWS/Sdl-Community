using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "ExportAnalysisReports wiki in the SDL Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5254/export-analysis-reports");
		}

		public override bool IsAvailable => true;
		public override string Category => "ExportAnalysisReports results";
		public override Icon Icon => PluginResources.Question;
	}
}