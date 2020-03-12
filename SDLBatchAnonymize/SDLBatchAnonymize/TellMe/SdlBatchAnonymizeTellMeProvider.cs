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
				Keywords = new []{ "sdl batch anonymize", "sdlbatchanonymize", "sdl batch anonymize community", "sdl batch anonymize support" }
			},
			new SdlBatchAnonymizeStoreAction
			{
				Keywords = new []{ "sdl batch anonymize", "sdl batch anonymize store", "sdlbatchanonymize store", "sdl batch anonymize store download" }
			}
		};
	}
}
