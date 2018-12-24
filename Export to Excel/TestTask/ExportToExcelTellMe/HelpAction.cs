using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace ExportToExcel.ExportToExcelTellMe
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
				"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3397.export-to-excel");
		}

		public override bool IsAvailable => true;

		public override string Category => "Export to excel results";

		public override Icon Icon => PluginResources.Question;
	}
}
