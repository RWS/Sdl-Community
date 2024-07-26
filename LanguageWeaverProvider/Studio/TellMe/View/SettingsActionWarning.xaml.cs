using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LanguageWeaverProvider.Studio.TellMe.View
{
	/// <summary>
	/// Interaction logic for SettingsActionWarning.xaml
	/// </summary>
	public partial class SettingsActionWarning : Window
	{
		readonly string _url;

		public SettingsActionWarning(string url)
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
