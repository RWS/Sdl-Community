using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FailSafeTask.TellMe
{
	[TellMeProvider]
	public class FailSafeTaskTellMeProvider : ITellMeProvider
	{
		public string Name => "Fail Safe Task Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new FailSafeTaskDocumantationAction
			{
				Keywords = new[] { "fail safe task", "fail safe task documentation", "documentation" }
			},
			new FailSafeTaskCommunityForumAction
			{
				Keywords = new[] { "fail safe task", "fail safe task community", "fail safe task support", "fail safe task forum" }
			},
			new FailSafeTaskSourceCodeAction
			{
				Keywords = new[] { "fail safe task", "fail safe task source code", "source", "code" }
			},
            new FailSafeTaskSettingsAction
            {
                Keywords = new[] { "fail safe task", "fail safe task settings", "settings" }
            }
        };
	}
}