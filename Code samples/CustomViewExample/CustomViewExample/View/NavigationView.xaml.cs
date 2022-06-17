using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace CustomViewExample.View
{
	/// <summary>
	/// Interaction logic for NavigationView.xaml
	/// </summary>
	public partial class NavigationView : UserControl, IUIControl
	{
		public NavigationView()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
		}
	}
}
