using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	public class RapidAddTermSourceCodeAction:AbstractTellMeAction
	{
		public RapidAddTermSourceCodeAction()
		{
			Name = "Rapid Add Term Source Code";
		}
		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/RapidAddTerm");
		}
		public override bool IsAvailable => true;
		public override string Category => "Rapid Add Terms results";
		public override Icon Icon => PluginResources.TellMe_SourceCode;
	}
}
