using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Qualitivity.TellMe.Actions
{
	public class SourceCodeAction : AbstractTellMeAction
	{
		public SourceCodeAction() => Name = $"{PluginResources.Plugin_Name} Source Code";

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.SourceCode;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/Qualitivity");
		}
	}
}