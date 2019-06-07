using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer
{
	[ViewPart(
		Id = "DSi Viewer",
		Name = "DSi Viewer",
		Description = "DSi Viewer",
		Icon = "ds")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class DsiViewerController : AbstractViewPartController
	{
		private readonly SdiControl _control = new SdiControl(); 
		protected override Control GetContentControl()
		{
			return _control;
		}

		protected override void Initialize()
		{
		}
	}
}
