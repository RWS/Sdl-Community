using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StudioViews.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] {"studio", "views", "studioviews", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "studio", "views", "studioviews", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "studio", "views", "studioviews", "source", "code" }

			}
		};
	}
}