using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.Actions
{
	[Action("XLIFFManager_Refresh_Action", 
		Name = "XLIFFManager_Refresh_Name",
		Description = "XLIFFManager_Refresh_Description",
		ContextByType = typeof(XLIFFManagerViewController),
		Icon = "Refresh"
		)]
	[ActionLayout(typeof(XLIFFManagerViewGroup), 3, DisplayType.Large)]
	public class RefreshAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		private PathInfo _pathInfo;
		private XLIFFManagerViewController _controller;

		protected override void Execute()
		{
			_controller.RefreshProjects();
		}

		
		public override void Initialize()
		{
			Enabled = true;

			_controller = SdlTradosStudio.Application.GetController<XLIFFManagerViewController>();
			_pathInfo = new PathInfo();
		}
	}
}
