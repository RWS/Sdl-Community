using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				Keywords = new[] {"mtenhanced", "mtenhanced store", "mtenhanced download"}
			},
			new MTEnhancedHelpAction
			{
				Keywords = new[] {"mtenhanced", "mtenhanced help", "mtenhanced guide"}
			},
			new MTEnhancedCommunityForumAction()
			{
				Keywords = new[] {"mtenhanced", "mtenhanced forum", "mtenhanced report"}
			}
		};
	}
}
