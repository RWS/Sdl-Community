using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATEDocumentationAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Question;

		public IATEDocumentationAction()
		{
			Name = "IATE Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/30?tab=documentation");
		}
	}
}