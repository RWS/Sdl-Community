using Sdl.Community.DsiViewer.Services;
using Sdl.Community.DsiViewer.View;
using Sdl.Community.DsiViewer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
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
        private readonly DsiViewerView _control = new();

        protected override IUIControl GetContentControl()
        {
            return _control;
        }

        protected override void Initialize()
        {
            var dataContext = new DsiViewerViewModel(new TqeReader());
            _control.DataContext = dataContext;
        }
    }
}