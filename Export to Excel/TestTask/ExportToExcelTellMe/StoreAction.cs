using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Properties;
using Sdl.TellMe.ProviderApi;

namespace ExportToExcel.ExportToExcelTellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download ExportToExcel";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/export-to-excel/532/");
		}

		public override bool IsAvailable => true;
		public override string Category => "Export to excel results";
		public override Icon Icon => PluginResources.Download_ExportToExcel;
	}
}
