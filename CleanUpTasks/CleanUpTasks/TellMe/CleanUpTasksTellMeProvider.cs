using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	[TellMeProvider]
    public class CleanUpTasksTellMeProvider : ITellMeProvider
    {
        public string Name => "CleanUpTasks Tell Me provider";

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
            new CleanUpTasksDocumentationAction
            {
                Keywords = new []{ "cleanup", "tasks", "cleanUpTasks", "documentation" }
            },
            new CleanUpTasksSupportAction
            {
                Keywords = new []{ "cleanup", "tasks", "cleanUpTasks community", "cleanUpTasks support", "forum" }
            },
            new CleanUpTasksSourceCodeAction
            {
                Keywords = new[] { "cleanup", "tasks", "cleanUpTasks", "source", "code" }
            }
            ,
            new CleanUpTasksSettingsAction
            {
                Keywords = new[] { "cleanup", "tasks", "cleanUpTasks", "settings" }
            }
        };
    }
}