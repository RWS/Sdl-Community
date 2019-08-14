using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer
{
	[ViewPart(
		Id = "DSI Viewer",
		Name = "DSI Viewer",
		Description = "DSI Viewer",
		Icon = "ds")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class DsiViewerController : AbstractViewPartController
	{
		private readonly DsiViewerControl _control = new DsiViewerControl(); 
		protected override Control GetContentControl()
		{
			return _control;
		}

		protected override void Initialize()
		{
		}
	}
}
