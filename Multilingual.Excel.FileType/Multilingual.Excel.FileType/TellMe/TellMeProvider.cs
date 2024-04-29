using Sdl.TellMe.ProviderApi;

namespace Multilingual.Excel.FileType.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions =>
		[
			new CommunityWikiAction
			{
				Keywords = ["multi", "multilingual", "excel", "filetype", "file", "type", "community", "support", "wiki"
				]
			},
			new CommunityAppStoreForumAction
			{
				Keywords = ["multi", "multilingual", "excel", "filetype", "file", "type", "support", "forum"]
			},
			new SourceCodeAction
			{
				Keywords = ["multi", "multilingual", "excel", "filetype", "file", "type", "source code"]
			},
			new SettingsAction
			{
				Keywords = ["multi", "multilingual", "excel", "filetype", "file", "type", "settings"]
			}

		];
	}
}
