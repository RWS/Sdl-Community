using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	public partial class TmAnonymizerLogViewControl : UserControl
	{
		public TmAnonymizerLogViewControl(MainViewModel model)
		{
			InitializeComponent();

			var window = new LogViewControl(model);

			window.InitializeComponent();
			elementHost.Child = window;
		}
	}
}
