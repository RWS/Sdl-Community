using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Sdl.Community.DeepLMTProvider.WPF.Ui
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings 
	{
		public Settings()
		{
			InitializeComponent();
		}

		private void Resend_OnChecked(object sender, RoutedEventArgs e)
		{
			
		}

		private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3266.deepl-mt-provider");
		}
	}
}
