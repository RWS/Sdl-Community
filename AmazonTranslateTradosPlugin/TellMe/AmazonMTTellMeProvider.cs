using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin.TellMe
{
	[TellMeProvider]
	public class AmazonMTTellMeProvider : ITellMeProvider
	{
		public string Name => "Amazon Translate MT Provider Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new AmazonMTProviderWikiAction
			{
				Keywords = new[] { "amazon translate mt provider", "amazon mt provider", "amazon", "amazon community", "amazon support", "amazon wiki" }
			},
			new AmazonMTProviderCommunityForumAction
			{
				Keywords = new[] { "amazon translate mt provider", "amazon mt provider", "amazon", "amazon community", "amazon support", "amazon forum" }
			},
			new AmazonMTProviderStoreAction
			{
				Keywords = new[] { "amazon translate mt provider", "amazon mt provider", "amazon", "amazon store", "amazon download", "amazon appstore" }
			}
		};
	}
}