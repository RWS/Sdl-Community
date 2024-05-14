using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	public class SourceCodeAction : AbstractTellMeAction
	{
		public SourceCodeAction()
		{
			Name = "StarTransit source code";
		}

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/StarTransit");
		}

		public override bool IsAvailable => true;
		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.SourceCode;
	}
}