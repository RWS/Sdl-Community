using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportToExcel.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "ExportToExcel wiki in the community";
		}

		public override void Execute()
		{
			Process.Start(
				"https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3397/export-to-excel");
		}

		public override bool IsAvailable => true;

		public override string Category => "Export to excel results";

		public override Icon Icon => PluginResources.Question;
	}
}
