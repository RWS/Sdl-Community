using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.ExportAnalysisReports.TellMe.Actions
{
	public class HelpAction : ExportAnalysisReportAbstractTellMeAction
	{
		public HelpAction()
		{
			Name = $"{PluginResources.Plugin_Name} Documentation";
		}

		public override Icon Icon => PluginResources.TellmeDocumentation;

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/92?tab=documentation");
		}
	}
}