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
	[Action("XLIFFManagerOpenSettingsAction", typeof(XLIFFManagerViewController), Name = " Settings ", Icon = "Settings", Description = "Settings ")]
	[ActionLayout(typeof(XLIFFManagerSettingsGroup), 7, DisplayType.Large)]
	public class OpenSettingsAction: AbstractViewControllerAction<XLIFFManagerViewController>
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
