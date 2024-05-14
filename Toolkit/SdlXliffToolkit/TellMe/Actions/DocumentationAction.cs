using System.Diagnostics;
using System.Drawing;

namespace SdlXliffToolkit.TellMe.Actions
{
	public class DocumentationAction : ToolkitAbastractTellMeAction
	{
		public DocumentationAction()
		{
			Name = $"{PluginResources.Plugin_Name} Documentation";
		}

		public override Icon Icon => PluginResources.TellmeDocumentation;

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/77?tab=documentation");
		}
	}
}