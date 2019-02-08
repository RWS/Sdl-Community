using Sdl.TellMe.ProviderApi;


namespace Sdl.Community.CleanUpTasks.TellMe
{
	[TellMeProvider]
	public class CleanUpTasksTellMeProvider	  : ITellMeProvider
	{
		public string Name => "CleanUpTasks tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CleanUpTasksStoreAction
			{
				Keywords = new []{ "cleanUpTasks", "cleanUpTasks store", "cleanUpTasks download" }
			},
			new CleanUpTasksSupportAction
			{
				Keywords = new []{ "cleanUpTasks", "cleanUpTasks community", "cleanUpTasks support" }
			},
			new CleanUpTasksHelpAction
			{
				Keywords = new[] { "cleanUpTasks", "cleanUpTasks help", "cleanUpTasks guide" } 
			}
		};

	}
}
