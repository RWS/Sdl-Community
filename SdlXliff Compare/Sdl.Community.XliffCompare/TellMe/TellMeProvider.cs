using Sdl.Community.XliffCompare.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XliffCompare.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "sdlxliff compare", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdlxliff compare", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "sdlxliff compare source code" }
			},
			new SdlXliffCompareAction
			{
				Keywords = new[] { "sdlxliff compare" }
			},
			new SettingsAction
			{
				Keywords = new[] { "sdlxliff compare settings" }
			}
		};
	}
}