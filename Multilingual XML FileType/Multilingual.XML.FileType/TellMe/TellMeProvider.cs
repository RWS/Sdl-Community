using Multilingual.XML.FileType.TellMe;
using Sdl.TellMe.ProviderApi;

namespace Multilingual.XML.FileType
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "multi", "multilingual", "xml", "filetype", "file", "type", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "multi", "multilingual", "xml", "filetype", "file", "type", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "multi", "multilingual", "xml", "filetype", "file", "type", "support", "forum" }
			},
			new SettingsAction
			{
				Keywords = new[] { "multi", "multilingual", "xml", "filetype", "file", "type", "support", "settings" }
			}
		};
	}
}
