using Sdl.TellMe.ProviderApi;

namespace ETSTranslationProvider.ETSTellMe
{
	[TellMeProvider]
	public class ETSTellMeProvider : ITellMeProvider
	{
		public string Name => "ETS tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new ETSMTAction
			{
				Keywords = new[] {"ets", "ets mt", "ets mt documentation" }
			},
			new ETSStoreAction
			{
				Keywords = new[] { "ets", "ets store", "ets download" }
			},
			new ETSTranslationServerAction
			{
				Keywords = new[] { "ets", "enterprise translation server", "enterprise translation server documentation", "enterprise translation server website" }
			},
			new ETSOfficialWebAction
			{
				Keywords = new[] { "ets", "ets official", "ets official web site", "ets official documentation" }
			},
			new ETSLanguagePairsAction
			{
				Keywords = new[] { "ets", "ets language pairs" }
			},
			new ETSCommunitySupportAction
			{
				Keywords = new[] {"ets", "ets community", "ets support" }
			}
		};
	}
}