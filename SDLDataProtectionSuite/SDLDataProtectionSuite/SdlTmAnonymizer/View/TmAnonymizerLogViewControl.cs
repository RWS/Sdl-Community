using System.Windows.Forms;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	public partial class TmAnonymizerLogViewControl : UserControl
	{
		public TmAnonymizerLogViewControl(MainViewModel model)
		{
			InitializeComponent();

			var window = new LogView(model);

			window.InitializeComponent();
			elementHost.Child = window;
		}
	}
}
