using System.Windows.Forms;

namespace Sdl.Community.SdiViewer
{
	public partial class SdiControl : UserControl
	{
		public SdiControl()
		{
			InitializeComponent();
			var viewModel = new SdiControlViewModel();
			var sdiControl = new SdiWpfControl
			{
				DataContext = viewModel
			};
			sdiControl.InitializeComponent();
			elementHost.Child = sdiControl;
		}
	}
}
