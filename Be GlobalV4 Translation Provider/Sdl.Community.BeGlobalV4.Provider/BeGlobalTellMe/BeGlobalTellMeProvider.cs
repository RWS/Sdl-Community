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
				Keywords = new []{ "beGlobal", "beGlobal community", "beGlobal support" }
			},
			new BeGlobalContactAction
			{
				Keywords = new []{ "beGlobal", "beGlobal contact", "beGlobal trial" }
			},
			new BeGlobalHelpAction
			{
				Keywords = new []{ "beGlobal", "beGlobal help", "beGlobal guide" }
			},
			new BeGlobalStoreAction
			{
				Keywords = new []{ "beGlobal", "beGlobal store", "beGlobal download" }
			}
		};
	}
}
