using Sdl.TellMe.ProviderApi;
using SdlXliffToolkit.TellMe.Actions;

namespace SdlXliffToolkit.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "sdlxliff", "toolkit", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdlxliff", "toolkit", "support", "forum" }
			},
			new ToolkitAction
			{
				Keywords = new[] { "sdlxliff toolkit settings" }
			},new SourceCodeAction
			{
				Keywords = new[] { "sdlxliff toolkit source code" }
			}
		};
	}
}
