using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RecordSourceTU.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "Record Source TU plugin wiki";
		}

		public override bool IsAvailable => true;

		public override string Category => "Record Source TU results";

		public override Icon Icon => PluginResources.Question;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/4720/recordsource-tu");
		}
	}
}