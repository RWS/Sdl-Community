using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	[TellMeProvider]
	public class CopyTagsTellMeProvider : ITellMeProvider
	{
		public string Name => "Trados Copy Tags Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CopyTagsCommunitySupportAction
			{
				Keywords = new []{ "tradoscopytags", "trados copy tags", "copy tags", "copy tags community", "copy tags support", "tradoscopytags community", "tradoscopytags support" }
			},
			new CopyTagsHelpAction
			{
				Keywords = new []{ "tradoscopytags", "trados copy tags", "copy tags", "copy tags help", "copy tags guide", "tradoscopytags help", "tradoscopytags guide" }
			},
			new CopyTagsStoreAction
			{
				Keywords = new []{ "tradoscopytags", "trados copy tags", "copy tags", "copy tags store", "copy tags download", "tradoscopytags store", "tradoscopytags download" }
			}
		};
	}
}
