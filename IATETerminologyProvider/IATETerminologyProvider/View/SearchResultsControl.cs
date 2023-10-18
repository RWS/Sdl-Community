using System.Windows.Forms;
using CefSharp.WinForms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.IATETerminologyProvider.View
{
	public partial class SearchResultsControl : UserControl, IUIControl
	{
		public SearchResultsControl()
		{
			InitializeComponent();
		}

		public ChromiumWebBrowser Browser => searchResultsWebBrowser;

	}
}
