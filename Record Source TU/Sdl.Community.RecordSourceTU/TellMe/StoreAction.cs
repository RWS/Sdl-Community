using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RecordSourceTU.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download Record Source TU from AppStore";
		}

		public override bool IsAvailable => true;

		public override string Category => "Record Source TU results";

		public override Icon Icon => PluginResources.Download;

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/record-source-tu/504/");
		}
	}
}