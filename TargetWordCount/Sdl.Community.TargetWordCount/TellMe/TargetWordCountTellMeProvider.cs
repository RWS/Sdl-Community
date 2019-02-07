using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TargetWordCount.TellMe
{
	[TellMeProvider]
	public class TargetWordCountTellMeProvider : ITellMeProvider
	{
		public string Name => "Target word count";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new TargetWordCountWikiAction
			{
				Keywords = new[] { "target word count", "target word count community", "target word count support", "target word count wiki" }
			},
			new TargetWordCountCommunityForumAction
			{
				Keywords = new[] { "target word count", "target word count community", "target word count support", "target word count forum" }
			},
			new TargetWordCountStoreAction
			{
				Keywords = new[] { "target word count", "target word count store", "target word count download", "target word count appstore" }
			}
		};
	}
}