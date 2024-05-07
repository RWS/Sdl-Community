using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TermExcelerator.TellMe
{
	public class ExcelTermDocumentationAction : AbstractTellMeAction
	{
		public ExcelTermDocumentationAction()
		{
			Name = $"{PluginResources.Plugin_Name} Documentation";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.TellmeDocumentation;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/59?tab=documentation");
		}
	}
}