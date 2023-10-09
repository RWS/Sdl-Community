using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.IATETerminologyProvider.View
{
	public partial class SearchResultsControl : UserControl, IUIControl
	{
		public SearchResultsControl()
		{
			InitializeComponent();
		}

		public WebView2 Browser => searchResultsWebBrowser;

	}
}
