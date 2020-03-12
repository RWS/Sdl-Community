using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	[TellMeProvider]
	public class CopyTagsTellMeProvider : ITellMeProvider
	{
		public string Name => "SDL Copy Tags tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CopyTagsCommunitySupportAction
			{
				Keywords = new []{ "sdlcopytags", "sdl copy tags", "copy tags", "copy tags community", "copy tags support", "sdlcopytags community", "sdlcopytags support" }
			},
			new CopyTagsHelpAction
			{
				Keywords = new []{ "sdlcopytags", "sdl copy tags", "copy tags", "copy tags help", "copy tags guide", "sdlcopytags help", "sdlcopytags guide" }
			},
			new CopyTagsStoreAction
			{
				Keywords = new []{ "sdlcopytags", "sdl copy tags", "copy tags", "copy tags store", "copy tags download", "sdlcopytags store", "sdlcopytags download" }
			}
		};
	}
}
