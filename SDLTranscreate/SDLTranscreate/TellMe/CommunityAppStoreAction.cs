using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Trados.Transcreate.TellMe
{
	public class CommunityAppStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format("{0} results", PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Download;

		public CommunityAppStoreAction()
		{
			Name = string.Format("Download {0} from the AppStore", PluginResources.Plugin_Name);
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/42");
		}
	}
}
