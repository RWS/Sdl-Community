using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLXLIFFSplitMerge.TellMe
{
	[TellMeProvider]
	public class SplitMergeTellMeProvider : ITellMeProvider
	{
		public string Name => "SDLXLIFF Split/Merge tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SplitMergeWikiAction
			{
				Keywords = new[] {"sdlxliff split merge", "split merge", "split/merge", "split merge community", "split/merge community", "split merge community support",
					"sdlxliff split merge community", "sdlxliff split/merge community", "sdlxliff split merge community support", "sdlxliff split/merge community support",
					"sdlxliff split merge wiki", "sdlxliff split/merge wiki", "split/merge community support", "split merge wiki", "split/merge wiki" }
			},
			new SplitMergeCommunityForumAction
			{
				Keywords = new[] { "sdlxliff split merge", "split merge", "split merge community", "split/merge community", "sdlxliff split merge community",
					"split merge support", "split/merge support", "split merge forum", "split/merge forum", "sdlxliff split/merge community",
					"sdlxliff split merge support", "sdlxliff split/merge support", "sdlxliff split merge forum", "sdlxliff split/merge forum"
				}
			},
			new SplitMergeStoreAction
			{
				Keywords = new[] { "sdlxliff split merge", "split merge", "split merge store", "sdlxliff split/merge store", "sdlxliff split merge store",
					"sdlxliff split/merge store", "split merge download", "split/merge download", "split merge appstore", "split/merge appstore",
					"sdlxliff split merge download", "sdlxliff split/merge download", "sdlxliff split merge appstore", "sdlxliff split/merge appstore"
				}
			}
		};
	}
}