using System.Windows.Forms;

namespace Sdl.Community.DsiViewer
{
	public partial class SdiControl : UserControl
	{
		public SdiControl()
		{
			InitializeComponent();
			var viewModel = new DsiControlViewModel();
			var sdiControl = new SdiWpfControl
			{
				DataContext = viewModel
			};
			sdiControl.InitializeComponent();
			elementHost.Child = sdiControl;
		}
	}
}
