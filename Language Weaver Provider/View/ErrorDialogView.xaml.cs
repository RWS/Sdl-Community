using System.Windows;
using System.Windows.Input;

namespace LanguageWeaverProvider.View
{
	/// <summary>
	/// Interaction logic for ErrorDialogView.xaml
	/// </summary>
	public partial class ErrorDialogView : Window
	{
		public ErrorDialogView()
		{
			InitializeComponent();
			Height = 200;
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