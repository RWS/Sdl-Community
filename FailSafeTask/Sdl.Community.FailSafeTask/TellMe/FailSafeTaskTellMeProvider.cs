using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FailSafeTask.TellMe
{
	public class FailSafeTaskTellMeProvider : ITellMeProvider
	{
		public string Name => "Fail safe task tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new FailSafeTaskCommunityWikiAction
			{
				Keywords = new[] { "fail safe task", "fail safe task community", "fail safe task support", "fail safe task wiki" }
			},
			new FailSafeTaskCommunityForumAction
			{
				Keywords = new[] { "fail safe task", "fail safe task community", "fail safe task support", "fail safe task forum" }
			},
			new FailSafeTaskStoreAction
			{
				Keywords = new[] { "fail safe task", "fail safe task store", "fail safe task download", "fail safe task appstore" }
			}
		};
	}
}