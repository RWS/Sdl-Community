using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StudioViews.TellMe
{
	public class CommunityAppStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		
		public override string Category => "Studio Views results";
		
		public override Icon Icon => PluginResources.Download;

		public CommunityAppStoreAction()
		{
			Name = "Download Studio Views from the AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/studioviews/1162");
		}
	}
}
