using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.IATETerminologyProvider.View
{
	[ViewPart(
		Id = "IATE Results Viewer",
		Name = "Search Results Viewer",
		Description = "IATE Search Results",
		Icon = "Iate_logo")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class SearchResultsViewerController : AbstractViewPartController
	{
		private readonly SearchResultsControl _control = new();

		public BrowserWindow Browser => _control.Browser;

		protected override IUIControl GetContentControl()
		{
			return _control;
		}

		protected override void Initialize()
		{
		}
	}
}