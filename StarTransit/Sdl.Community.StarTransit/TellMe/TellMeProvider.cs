using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "StarTransit tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ "startransit", "star transit", "startransit community", "startransit support"}
			},
			new HelpAction
			{
				Keywords = new []{ "startransit", "star transit", "startransit help", "startransit guide" }
			},
			new StoreAction
			{
				Keywords = new []{ "startransit", "star transit", "startransit store", "startransit download" }
			}
		};
	}
}