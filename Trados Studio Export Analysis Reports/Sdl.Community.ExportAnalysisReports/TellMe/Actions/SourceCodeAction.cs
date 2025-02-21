using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.ExportAnalysisReports.TellMe.Actions
{
	public class SourceCodeAction : ExportAnalysisReportAbstractTellMeAction
	{
		public SourceCodeAction()
		{
			Name = $"{PluginResources.Plugin_Name} Source Code";
		}

		public override Icon Icon => PluginResources.SourceCode;

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/Export%20Analysis%20Reports");
		}
	}
}