using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public class DocumentationAction : AbstractTellMeAction
	{
		public DocumentationAction()
		{
			Name = "Trados Export Analysis Reports documentation";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/92?tab=documentation");
		}

		public override bool IsAvailable => true;
		public override string Category => "Trados Export Analysis Reports results";
		public override Icon Icon => PluginResources.TellmeDocumentation;
	}
}