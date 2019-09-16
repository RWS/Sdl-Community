using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AdvancedDisplayFilter.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download Advanced Display Filter";
		}

		public override string Category => "Community Advanced Display Filter results";

		public override Icon Icon => PluginResources.Download;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/community-advanced-display-filter/849/");
		}
	}
}