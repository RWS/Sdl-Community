using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATESourceCode : AbstractTellMeAction
	{
		public IATESourceCode()
		{
			Name = "IATE Source Code";
		}

		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.SourceCode;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/IATETerminologyProvider");
		}
	}
}