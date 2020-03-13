using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLBatchAnonymize.TellMe
{
	[TellMeProvider]
	public class SdlBatchAnonymizeTellMeProvider: ITellMeProvider
	{
		public string Name => "SDL BatchAnonymize tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SdlBatchAnonymizeCommunitySupportAction
			{
				Keywords = new []{ "sdl batch anonymizer", "sdlbatchanonymizer", "sdl batch anonymizer community", "sdl batch anonymizer support" }
			},
			new SdlBatchAnonymizeStoreAction
			{
				Keywords = new []{ "sdl batch anonymizer", "sdl batch anonymizer store", "sdlbatchanonymizer store", "sdl batch anonymizer store download" }
			},
			new SdlBatchAnonymizerWikiAction
			{
				Keywords = new []{ "sdl batch anonymizer", "sdl batch anonymizer wiki"}
			}
		};
	}
}
