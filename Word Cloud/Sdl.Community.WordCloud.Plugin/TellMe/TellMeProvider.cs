using Sdl.Community.WordCloud.Plugin.TellMe;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.WordCloud.Plugin
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityDocumentationAction
			{
				Keywords = new[] { "trados","studio" ,"word", "cloud", "documentation" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "trados", "studio", "word", "cloud", "support", "forum" }
			},
			new CommunitySourceCodeAction
			{
				Keywords = new[] { "trados", "studio", "word", "cloud", "source", "code" }
			},
            new SettingsAction
            {
                Keywords = new[] { "trados", "studio", "word", "cloud", "settings" }
            }
        };
	}
}
