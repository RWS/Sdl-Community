using System.Windows;
using MultiTermIX;

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
			var app = new ApplicationClass();
			app.ServerRepository.Location = "";
			app.ServerRepository.Connect("user", "password");

			var termbases = app.ServerRepository.Termbases;
			app.ServerRepository.Disconnect();

			//create local termbase
			app.LocalRepository.Connect("", "");
			var localTermbases = app.LocalRepository.Termbases;
			localTermbases.New("testT", "ss", "", @"C:\Users\[username]\Desktop\test.sdltb");
		}
	}
}
