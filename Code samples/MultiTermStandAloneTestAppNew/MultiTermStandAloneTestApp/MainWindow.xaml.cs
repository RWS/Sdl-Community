using System.Windows;
using Sdl.MultiTerm.Client.Api;

namespace MultiTermStandAloneTestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var app = new ServerRepository();
			app.Connect("", "", "");

			var oTbs = app.Termbases;
		}
	}
}