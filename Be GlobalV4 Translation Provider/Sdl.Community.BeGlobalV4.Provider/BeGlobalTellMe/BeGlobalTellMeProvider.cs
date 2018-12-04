using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	[TellMeProvider]
	public class BeGlobalTellMeProvider : ITellMeProvider
	{
		public string Name => "BeGlobal tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new BeGlobalCommunitySupportAction
			{
				Keywords = new []{ "beglobal", "beglobal community", "beglobal support" }
			},
			new BeGlobalContactAction
			{
				Keywords = new []{ "beglobal", "beglobal contact", "beglobal trial" }
			},
			new BeGlobalHelpAction
			{
				Keywords = new []{ "beglobal", "beglobal help", "beglobal guide" }
			},
			new BeGlobalStoreAction
			{
				Keywords = new []{ "beglobal", "beglobal store", "beglobal download" }
			},
		};
	}
}
