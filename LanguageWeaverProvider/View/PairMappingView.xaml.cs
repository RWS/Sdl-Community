using System.Windows;
using System.Windows.Input;

namespace LanguageWeaverProvider.View
{
	/// <summary>
	/// Interaction logic for PairMappingView.xaml
	/// </summary>
	public partial class PairMappingView : Window
	{
		public PairMappingView()
		{
			InitializeComponent();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Window.GetWindow(this) is not Window window)
			{
				return;
			}

			window.DragMove();
		}
	}
}