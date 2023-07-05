using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
    public class AppStoreDownloadAction : AbstractTellMeAction
    {
        public override bool IsAvailable => true;

        public override string Category => "StarTransit results";

        public override Icon Icon => PluginResources.Download;

        public AppStoreDownloadAction()
        {
            Name = string.Format("Download {0} from the AppStore", "StarTransit");
        }

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/45");
        }
    }
}
