using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.ExportAnalysisReports.TellMe.Actions
{
	public class CommunitySupportAction : ExportAnalysisReportAbstractTellMeAction
	{
		public CommunitySupportAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override Icon Icon => PluginResources.Question;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}