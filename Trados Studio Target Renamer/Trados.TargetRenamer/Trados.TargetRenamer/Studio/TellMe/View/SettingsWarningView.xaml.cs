using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Trados.TargetRenamer.Studio.TellMe.View
{
	/// <summary>
	/// Interaction logic for SettingsWarningView.xaml
	/// </summary>
	public partial class SettingsWarningView : Window
	{
		readonly string _url;

		public SettingsWarningView(string url)
		{
			InitializeComponent();
			_url = url;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (GetWindow(this) is Window window)
			{
				window.DragMove();
			}
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				Close();
			}
		}

		private void OpenUrl_KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter
			 || e.Key == Key.Space)
			{
				Process.Start(_url);
			}
		}

		private void OpenUrl_ButtonClicked(object sender, RoutedEventArgs e)
		{
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OpenUrl_ButtonClicked(object sender, MouseButtonEventArgs e)
		{
			Process.Start(_url);
		}
	}
}