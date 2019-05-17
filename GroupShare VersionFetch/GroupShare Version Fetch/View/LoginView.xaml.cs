namespace Sdl.Community.GSVersionFetch.View
{
	/// <summary>
	/// Interaction logic for LoginView.xaml
	/// </summary>
	public partial class LoginView 
	{
		public LoginView()
		{
			InitializeComponent();
			Loaded += LoginView_Loaded;
		}

		private void LoginView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			Loaded -= LoginView_Loaded;
		}
	}
}
