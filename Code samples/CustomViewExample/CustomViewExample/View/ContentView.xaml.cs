using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace CustomViewExample.View
{
	/// <summary>
	/// Interaction logic for ContentView.xaml
	/// </summary>
	public partial class ContentView : UserControl, IUIControl
	{
		public ContentView()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
		}
	}
}
