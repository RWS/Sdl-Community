using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.NumberVerifier.TellMe
{
	public class NumVerifierSourceCodeAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Number Verifier results";
		public override Icon Icon => PluginResources.SourceCode;

		public NumVerifierSourceCodeAction()
		{
			Name = string.Format("{0} Source Code", PluginResources.Plugin_Name);
		}

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/Number%20Verifier");
		}
	}
}