using System.Diagnostics;
using System.Windows.Navigation;

namespace Sdl.Community.DeepLMTProvider.WPF.Ui
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login 
	{
		public Login()
		{
			InitializeComponent();
		}

		private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start("https://www.deepl.com/api-contact.html");
		}
		
	}
}
