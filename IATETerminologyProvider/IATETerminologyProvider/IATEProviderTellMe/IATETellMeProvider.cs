using Sdl.TellMe.ProviderApi;

namespace IATETerminologyProvider.IATEProviderTellMe
{
	[TellMeProvider]
	public class IATETellMeProvider : ITellMeProvider
	{
		public string Name => "IATE tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new IATECommunitySupportAction
			{
				Keywords = new[] {"iate", "iate community", "iate support"}
			},
			new IATEStoreAction
			{
				Keywords = new[] {"iate", "iate store", "iate download"}
			},
			new IATEContactAction
			{
				Keywords = new[] {"iate", "iate contact", "iate official", "iate website", "iate web search"}
			},
		};
	}
}