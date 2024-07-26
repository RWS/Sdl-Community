using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AntidoteVerifier.TellMe
{
	public class SourceCodeAction : AbstractTellMeAction
	{
		public SourceCodeAction()
		{
			Name = "Antidote Verifier Source Code";
		}

		public override Icon Icon => PluginResources.ForumIcon;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start(
				"https://github.com/RWS/Sdl-Community/tree/c127e49af6513e8d158cd0b7d11b8630de0bbe6c/Antidote%20Verifier/Sdl.Community.AntidoteVerifier");
		}
	}
}