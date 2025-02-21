using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XLIFF.Manager.Actions
{
	[Action("XLIFFManager_OpenSettings_Action", typeof(XLIFFManagerViewController),
		Name = "XLIFFManager_Settings_Name",
		Icon = "Settings",
		Description = "XLIFFManager_Settings_Description")]
	[ActionLayout(typeof(XLIFFManagerSettingsGroup), 7, DisplayType.Large)]
	public class OpenSettingsAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		public override void Initialize() => Enabled = true;

		protected override void Execute() => ApplicationInstance.ShowSettingsWindow();
	}
}