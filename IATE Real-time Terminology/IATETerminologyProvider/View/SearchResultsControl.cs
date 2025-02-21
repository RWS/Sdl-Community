using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.IATETerminologyProvider.View
{
	public partial class SearchResultsControl : UserControl, IUIControl
	{
		public SearchResultsControl()
		{
			var elementHost = new ElementHost
			{
				Dock = DockStyle.Fill,
			};

			Controls.Add(elementHost);
			elementHost.Child = Browser;

			InitializeComponent();
		}

		public BrowserWindow Browser { get; set; } = new();
	}
}