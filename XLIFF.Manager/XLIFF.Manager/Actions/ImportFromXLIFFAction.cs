using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XLIFF.Manager.Actions
{
	[Action("XLIFFManager_ImportFromXLIFF_Action", typeof(XLIFFManagerViewController), 
		Name = "XLIFFManager_ImportFromXLIFF_Name", 
		Icon = "ImportFrom", 
		Description = "XLIFFManager_ImportFromXLIFF_Description")]
	[ActionLayout(typeof(XLIFFManagerActionsGroup), 5, DisplayType.Large)]
	public class ImportFromXLIFFAction : AbstractViewControllerAction<XLIFFManagerViewController>
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
