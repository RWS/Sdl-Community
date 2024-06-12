using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	[TellMeProvider]
	public class CopyTagsTellMeProvider : ITellMeProvider
	{
		public string Name => "Trados Copy Tags Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CopyTagsDocumentationAction
			{
				Keywords = new []{ "tradoscopytags", "copy", "tags", "documentation", "information" }
			},
			new CopyTagsCommunitySupportAction
            {
				Keywords = new []{ "tradoscopytags", "copy", "tags", "help", "community", "forum" }
			},
			new CommunitySourceCodeAction
            {
				Keywords = new []{ "tradoscopytags", "copy", "tags", "source code" }
			},
            new CopyTagsSettingsAction
            {
                Keywords = new []{ "tradoscopytags", "copy", "tags", "settings" }
            }
        };
	}
}
