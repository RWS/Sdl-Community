using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XLIFF.Manager.Actions
{

	[Action("XLIFFManagerExportToXLIFFAction", typeof(XLIFFManagerViewController), Name = " Export", Icon = "ExportTo", Description = "Export to XLIFF")]
	[ActionLayout(typeof(XLIFFManagerActionsGroup), 6, DisplayType.Large)]
	public class ExportToXLIFFAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		protected override void Execute()
		{
			MessageBox.Show("TODO");
		}

		public override void Initialize()
		{
			Enabled = true;
		}
	}
}
