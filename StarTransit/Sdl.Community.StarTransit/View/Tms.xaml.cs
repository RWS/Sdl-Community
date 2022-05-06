using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.StarTransit.View
{
	/// <summary>
	/// Interaction logic for Tms.xaml
	/// </summary>
	public partial class Tms
	{
		public Tms()
		{
			InitializeComponent();
		}

		private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!(sender is ScrollViewer scv)) return;
			scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
			e.Handled = true;
		}
	}
}
