using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public class SourceCodeAction : AbstractTellMeAction
	{
		public SourceCodeAction()
		{
			Name = "Export Analysis Reports source code";
		}

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/Export%20Analysis%20Reports");
		}

		public override bool IsAvailable => true;
		public override string Category => "Trados Export Analysis Reports results";
		public override Icon Icon => PluginResources.SourceCode;
	}
}