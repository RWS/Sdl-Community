using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FileTypeSupport.MXLIFF.TellMe
{
	[TellMeProvider]
	public class MXLIFFTellMeProvider : ITellMeProvider
	{
		public string Name => "MXLIFF tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new MXLIFFCommunityWikiAction
			{
				Keywords = new[] {"mxliff", "mxliff community", "mxliff support", "mxliff wiki" }
			},
			new MXLIFFCommunityForumAction
			{
				Keywords = new[] { "mxliff", "mxliff community", "mxliff support", "mxliff forum" }
			},
			new MXLIFFStoreAction
			{
				Keywords = new[] {"mxliff", "mxliff store", "mxliff download", "mxliff appstore" }
			}
		};
	}
}