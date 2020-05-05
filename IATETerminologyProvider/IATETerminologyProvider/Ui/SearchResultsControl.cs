using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace IATETerminologyProvider.Ui
{
	public partial class SearchResultsControl : UserControl,IUIControl
	{
		public SearchResultsControl()
		{
			InitializeComponent();
		}

		public WebBrowser Browser => searchResultsWebBrowser;
	}
}
