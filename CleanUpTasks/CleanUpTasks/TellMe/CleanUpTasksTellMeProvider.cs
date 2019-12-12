using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	[TellMeProvider]
	public class CleanUpTasksTellMeProvider	  : ITellMeProvider
	{
		public string Name => "CleanUpTasks tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CleanUpTasksStoreAction
			{
				Keywords = new []{ "cleanup", "cleanUpTasks store", "cleanUpTasks download" }
			},
			new CleanUpTasksSupportAction
			{
				Keywords = new []{ "cleanup", "cleanUpTasks community", "cleanUpTasks support" }
			},
			new CleanUpTasksHelpAction
			{
				Keywords = new[] { "cleanup", "cleanUpTasks help", "cleanUpTasks guide" } 
			}
		};

	}
}
