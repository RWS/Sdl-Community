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

		public static bool IsInitialized { get; set; }

		public BrowserWindow Browser => _control.Browser;

		public string Url { get; set; }

		public void NavigateTo(string url)
		{
			Url = url;
			if (!IsVisible) Show();
			Navigate();
		}

		protected override IUIControl GetContentControl() => _control;

		protected override async void Initialize()
		{
			IsInitialized = true;
			await Browser.InitializeWebView();
		}

		private void Navigate() => Browser.Navigate(Url);
	}
}