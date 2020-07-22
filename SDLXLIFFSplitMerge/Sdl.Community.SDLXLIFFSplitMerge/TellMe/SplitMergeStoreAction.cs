using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLXLIFFSplitMerge.TellMe
{
	public class SplitMergeStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDLXLIFF Split/Merge results";
		public override Icon Icon => PluginResources.Download;

		public SplitMergeStoreAction()
		{
			Name = "Download SDLXLIFF Split/Merge from the AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/sdlxliff-split-merge/20/");
		}
	}
}