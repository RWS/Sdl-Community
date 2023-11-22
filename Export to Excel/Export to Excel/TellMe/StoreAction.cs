using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportToExcel.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download ExportToExcel";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/27");
		}

		public override bool IsAvailable => true;
		public override string Category => "Export to excel results";
		public override Icon Icon => PluginResources.Download_ExportToExcel;
	}
}
