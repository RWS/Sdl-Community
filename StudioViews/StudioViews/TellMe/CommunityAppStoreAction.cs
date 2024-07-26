using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StudioViews.TellMe
{
	public class CommunityAppStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Download;

		public CommunityAppStoreAction()
		{
			Name = PluginResources.TellMe_Download_StudioViews_From_AppStore;
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/12");
		}
	}
}
