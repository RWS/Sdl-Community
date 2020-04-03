using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download ExportAnalysisReports from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/transitpackage-handler/573/");
		}

		public override bool IsAvailable => true;
		public override string Category => "ExportAnalysisReports results";
		public override Icon Icon => PluginResources.Download;
	}
}