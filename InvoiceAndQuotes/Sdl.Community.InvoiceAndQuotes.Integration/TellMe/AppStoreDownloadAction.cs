using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InvoiceAndQuotes.Integration.TellMe
{
	public class AppStoreDownloadAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Download;

		public AppStoreDownloadAction()
        {
            Name = string.Format("Download {0} from the AppStore", PluginResources.Plugin_Name);
        }

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/55");
		}
	}
}
