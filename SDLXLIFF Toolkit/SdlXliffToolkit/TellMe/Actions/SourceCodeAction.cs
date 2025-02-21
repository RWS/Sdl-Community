using System.Diagnostics;
using System.Drawing;

namespace SdlXliffToolkit.TellMe.Actions
{
	public class SourceCodeAction : ToolkitAbastractTellMeAction
	{
		public SourceCodeAction()
		{
			Name = $"{PluginResources.Plugin_Name} Source Code";
		}

		public override Icon Icon => PluginResources.SourceCode;

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/Toolkit");
		}
	}
}