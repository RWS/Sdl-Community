using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for AcceptWindow.xaml
	/// </summary>
	public partial class TellMe 
	{
		readonly string _url;

		public TellMe(string url)
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