using System.Windows;
using System.Windows.Input;

namespace LanguageWeaverProvider.View
{
	/// <summary>
	/// Interaction logic for CredentialsMainView.xaml
	/// </summary>
	public partial class CredentialsMainView : Window
	{
		public CredentialsMainView()
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