using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "MT Enhanced tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new StoreAction
			{
				Keywords = new[] {"mt enhanced", "mtenhanced store", "mtenhanced download"}
			},
			new HelpAction
			{
				Keywords = new[] {"mt enhanced", "mtenhanced help", "mtenhanced guide"}
			},
			new CommunityForumAction()
			{
				Keywords = new[] {"mt enhanced", "mtenhanced forum", "mtenhanced report"}
			},
			new SettingsAction()
			{
				Keywords = new[] {"mt enhanced", "mtenhanced settings", "mtenhanced edit"}
			}
		};
	}
}