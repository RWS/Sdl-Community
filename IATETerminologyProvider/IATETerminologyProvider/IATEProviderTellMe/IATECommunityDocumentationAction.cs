using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATECommunityDocumentationAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Question;

		public IATECommunityDocumentationAction()
		{
			Name = "IATE Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3390/iate-real-time-terminology");
		}
	}
}