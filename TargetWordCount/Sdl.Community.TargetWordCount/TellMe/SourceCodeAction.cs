using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TargetWordCount.TellMe
{
	public class SourceCodeAction : AbstractTellMeAction
	{
		public SourceCodeAction() => Name = $"{PluginResources.Plugin_Name} Source Code";

		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.SourceCode;
		public override bool IsAvailable => true;

		public override void Execute() =>
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/TargetWordCount");
	}
}