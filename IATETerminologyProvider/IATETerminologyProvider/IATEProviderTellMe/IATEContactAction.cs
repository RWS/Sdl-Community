using System;
using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace IATETerminologyProvider.IATEProviderTellMe
{
	public class IATEContactAction : AbstractTellMeAction
	{
		public IATEContactAction()
		{
			Name = "IATE official web site terminology";
		}

		public override bool IsAvailable => throw new NotImplementedException();
		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Iate_logo;

		public override void Execute()
		{
			Process.Start("https://iate.europa.eu/home");
		}
	}
}