using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	public class ApplyTMSourceCodeAction : AbstractTellMeAction
	{
		public ApplyTMSourceCodeAction()
		{
			Name = "Apply TM Template Source Code";
		}

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/ApplyTMTemplate");
		}

		public override bool IsAvailable => true;

		public override string Category => "Apply TM Template results";

		public override Icon Icon => PluginResources.TellMe_SourceCode;
	}
}