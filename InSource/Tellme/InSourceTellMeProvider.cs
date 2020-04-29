using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InSource.Tellme
{
	[TellMeProvider]
	public class InSourceTellMeProvider : ITellMeProvider
	{
		public string Name => "InSource tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new InSourceCommunitySupportAction
			{
				Keywords = new[] {"insource", "insource community", "insource support"}
			},
			new InSourceHelpAction
			{
				Keywords = new[] {"insource", "insource help", "insource guide"}
			},
			new InSourceStoreAction
			{
				Keywords = new[] {"insource", "insource store", "insource download"}
			}
		};
	}
}
