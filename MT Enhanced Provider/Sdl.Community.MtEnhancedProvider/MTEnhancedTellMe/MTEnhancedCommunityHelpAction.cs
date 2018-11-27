using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MtEnhancedProvider.MTEnhancedTellMe
{
	public class MTEnhancedCommunityForumAction : AbstractTellMeAction
	{
		public MTEnhancedCommunityForumAction()
		{
			Name = "AppStore help";
		}

		public override void Execute()
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/f/160");
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";
	}
}
