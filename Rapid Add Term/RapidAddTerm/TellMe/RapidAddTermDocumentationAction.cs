using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	public class RapidAddTermDocumentationAction : AbstractTellMeAction
	{
		public RapidAddTermDocumentationAction()
		{
			Name = "Rapid Add Term Documentation";
		}
		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/35?tab=documentation");
		}
		public override bool IsAvailable => true;
		public override string Category => "Rapid Add Terms results";
		public override Icon Icon => PluginResources.TellMe_Documentation;
	}
}
