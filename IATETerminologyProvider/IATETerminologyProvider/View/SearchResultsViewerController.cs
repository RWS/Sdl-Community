using Microsoft.Web.WebView2.Wpf;
using Sdl.Community.IATETerminologyProvider.ViewModel;
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
		private SearchResultsControl _control;
		private BrowserViewModel _browserViewModel;
		protected override void Initialize()
		{
		}
		protected override IUIControl GetContentControl()
		{
			if (_control != null)
			{
				return _control;
			}

			_browserViewModel = new BrowserViewModel
			{
				Uri = null,
				RedirectUri = null,
				NeedsClearingCookies = false
			};

			_control = new SearchResultsControl
			{
				DataContext = _browserViewModel
			};

			return _control;
		}
		//public string _url;
		//public string _redirectUri;
		public WebView2 Browser=>_control.Browser;
	}
}
