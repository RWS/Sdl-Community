using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.IATETerminologyProvider.View
{
	public partial class SearchResultsControl : UserControl
	{
		public SearchResultsControl()
		{
			InitializeComponent();
		}

		public WebBrowser Browser => searchResultsWebBrowser;
	}
}
