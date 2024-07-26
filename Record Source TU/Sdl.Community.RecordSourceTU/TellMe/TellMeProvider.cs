using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RecordSourceTU.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "Record Source TU Tell Me Provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction() {Keywords = new[] {"record source tu", "support", "forum"}},
			new HelpAction{Keywords = new []{"record source tu help guide"}},
			new StoreAction{Keywords = new []{"record source tu store download"}}
		};
	}
}