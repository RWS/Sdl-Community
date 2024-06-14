using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLTM.Import.TellMe
{
	public class SdlTmImportSourceCodeAction: AbstractTellMeAction
	{
		public SdlTmImportSourceCodeAction()
		{
			Name = "SDLTM Import Plus Source Code";
		}
		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/SDLTM.Import");
		}
		public override bool IsAvailable => true;
		public override string Category => "SDLTM Import Plus results";
		public override Icon Icon => PluginResources.TellMe_SourceCode;
	}
}
