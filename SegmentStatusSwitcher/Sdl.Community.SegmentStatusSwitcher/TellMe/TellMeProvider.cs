using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SegmentStatusSwitcher.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new AppStoreDocumentationAction
			{
				Keywords = new[] { "segment", "status", "switcher", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "segment", "status", "switcher", "support", "forum" }
			},
			new CommunitySourceCodeAction
			{
				Keywords = new[] { "segment", "status", "switcher", "source", "code", "source code" }
			},
			new SettingsAction()
			{
				Keywords = new[] { "segment", "status", "switcher", "settings" }
			}
		};
	}
}
