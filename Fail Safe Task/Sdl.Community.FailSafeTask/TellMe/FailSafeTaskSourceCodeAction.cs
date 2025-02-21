using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FailSafeTask.TellMe
{
	public class FailSafeTaskSourceCodeAction : AbstractTellMeAction
	{	
		public override bool IsAvailable => true;
		public override string Category => "Fail Safe Task results";
		public override Icon Icon => PluginResources.TellMe_SourceCode;

		public FailSafeTaskSourceCodeAction()
		{
			Name = "Fail Safe Task Source Code";
		}

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/FailSafeTask");
		}
	}
}