using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATEContactAction : AbstractTellMeAction
	{
		public IATEContactAction()
		{
			Name = "IATE Official Website Terminology";
		}

		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Logo;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://iate.europa.eu/home");
		}
	}
}