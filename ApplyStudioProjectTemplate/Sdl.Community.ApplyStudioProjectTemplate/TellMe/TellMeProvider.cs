using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "apply", "studio", "project", "documentation" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "apply", "studio", "project", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "apply", "studio","project", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "apply", "studio","project", "settings" }
			}
		};
	}
}
