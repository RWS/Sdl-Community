using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	public class DocumentationAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.TellmeDocumentation;

		public DocumentationAction()
		{
			Name = string.Format("{0} Documentation", PluginResources.Plugin_Name);
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/39?tab=documentation");
		}
	}
}