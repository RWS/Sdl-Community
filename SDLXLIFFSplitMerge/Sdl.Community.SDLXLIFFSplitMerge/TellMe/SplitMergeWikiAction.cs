using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLXLIFFSplitMerge.TellMe
{
	public class SplitMergeWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDLXLIFF Split/Merge results";
		public override Icon Icon => PluginResources.Question;

		public SplitMergeWikiAction()
		{
			Name = "SDLXLIFF Split/Merge wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3166/sdlxliff-split-merge");
		}
	}
}