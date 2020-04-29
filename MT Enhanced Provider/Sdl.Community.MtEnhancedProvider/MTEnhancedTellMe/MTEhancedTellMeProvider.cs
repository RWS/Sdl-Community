using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MtEnhancedProvider.MTEnhancedTellMe
{
	[TellMeProvider]
	public class MTEhancedTellMeProvider : ITellMeProvider
	{
		public string Name => "MT Enhanced tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new MTEnhancedStoreAction
			{
				Keywords = new[] {"mt enhanced", "mtenhanced store", "mtenhanced download"}
			},
			new MTEnhancedHelpAction
			{
				Keywords = new[] {"mt enhanced", "mtenhanced help", "mtenhanced guide"}
			},
			new MTEnhancedCommunityForumAction()
			{
				Keywords = new[] {"mt enhanced", "mtenhanced forum", "mtenhanced report"}
			},
			new MTEnhancedSettingsAction()
			{
				Keywords = new[] {"mt enhanced", "mtenhanced settings", "mtenhanced edit"}
			}
		};
	}
}
