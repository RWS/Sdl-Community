using Sdl.Community.YourProductivity.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.YourProductivity.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "your", "productivity", "community", "support", "documentation", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "your", "productivity", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "your", "productivity", "source code" }
			},
			new ProductivityAction
			{
				Keywords = new[] { "your", "productivity" }
			}
		};
	}
}
