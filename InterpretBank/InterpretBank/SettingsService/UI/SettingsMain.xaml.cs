using System.Windows;
using System.Windows.Input;

namespace InterpretBank.SettingsService.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SettingsMain : Window
	{
		public SettingsMain()
		{
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}


		private void ClearFilepathButton(object sender, RoutedEventArgs e)
		{
			FilepathTextBox.Clear();
		}
	}
}
