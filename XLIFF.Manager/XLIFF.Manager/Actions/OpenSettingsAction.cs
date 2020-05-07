using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XLIFF.Manager.Actions
{
	[Action("XLIFFManager_OpenSettings_Action", typeof(XLIFFManagerViewController), 
		Name = "XLIFFManager_Settings_Name", 
		Icon = "Settings", 
		Description = "XLIFFManager_Settings_Description")]
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
