using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DsiViewer.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "DSI Viewer tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ "dsi viewer metadata community support", "dsi", "viewer" }
			},
			new HelpAction
			{
				Keywords = new []{ "dsi viewer metadata help guide", "dsi", "viewer" }
			},
			new StoreAction
			{
				Keywords = new []{ "dsi viewer metadata store download", "dsi", "viewer" }
			}
		};
	}
}