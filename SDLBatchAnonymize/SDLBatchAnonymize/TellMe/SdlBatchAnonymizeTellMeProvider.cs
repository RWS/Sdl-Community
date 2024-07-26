using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLBatchAnonymize.TellMe
{
	[TellMeProvider]
	public class SdlBatchAnonymizeTellMeProvider: ITellMeProvider
	{
		public string Name => "Trados BatchAnonymize Tell Me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SdlBatchAnonymizeCommunitySupportAction
			{
				Keywords = new []{ "trados batch anonymizer", "tradosbatchanonymizer", "trados batch anonymizer community", "trados batch anonymizer support" }
			},
			new SdlBatchAnonymizeStoreAction
			{
				Keywords = new []{ "trados batch anonymizer", "trados batch anonymizer store", "tradosbatchanonymizer store", "trados batch anonymizer store download" }
			},
			new SdlBatchAnonymizerWikiAction
			{
				Keywords = new []{ "trados batch anonymizer", "trados batch anonymizer wiki"}
			}
		};
	}
}
