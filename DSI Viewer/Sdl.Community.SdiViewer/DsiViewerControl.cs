using System.Windows.Forms;
using Sdl.Community.DsiViewer.ViewModel;

namespace Sdl.Community.DsiViewer
{
	public partial class DsiViewerControl : UserControl
	{
		public DsiViewerControl()
		{
			InitializeComponent();

			var viewModel = new DsiViewerViewModel();
			var sdiControl = new View.DsiViewerView
			{
				DataContext = viewModel
			};

			sdiControl.InitializeComponent();
			elementHost.Child = sdiControl;
		}
	}
}
