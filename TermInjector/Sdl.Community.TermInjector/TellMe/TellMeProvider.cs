using Sdl.Community.TermInjector.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TermInjector.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SettingsAction
			{
				Keywords = new[] {"term", "injector", "terminjector", "settings" }
			},new DocumentationAction
			{
				Keywords = new[] {"term", "injector", "terminjector", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "term", "injector", "terminjector", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "term", "injector", "terminjector", "source code" }}
		};
	}
}