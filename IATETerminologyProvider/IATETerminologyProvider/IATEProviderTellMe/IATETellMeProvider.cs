using Sdl.TellMe.ProviderApi;

namespace IATETerminologyProvider.IATEProviderTellMe
{
	[TellMeProvider]
	public class IATETellMeProvider : ITellMeProvider
	{
		public string Name => "IATE tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new IATECommunityWikiAction
			{
				Keywords = new[] {"iate", "iate community", "iate support", "iate wiki" }
			},
			new IATECommunityForumAction
			{
				Keywords = new[] {"iate", "iate community", "iate support", "iate forum" }
			},
			new IATEStoreAction
			{
				Keywords = new[] {"iate", "iate store", "iate download", "iate appstore" }
			},
			new IATEContactAction
			{
				Keywords = new[] {"iate", "iate contact", "iate official", "iate website", "iate web search"}
			}
		};
	}
}