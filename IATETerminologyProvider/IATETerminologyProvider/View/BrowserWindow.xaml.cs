using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;
using UserControl = System.Windows.Controls.UserControl;

namespace Sdl.Community.IATETerminologyProvider.View
{
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : UserControl
	{
		public BrowserWindow()
		{
			InitializeComponent();
			InitializeWebView();
		}

		public async Task InitializeWebView()
		{
			if (WebView2.CoreWebView2 == null)
			{
				WebView2.CreationProperties = new CoreWebView2CreationProperties
				{
					UserDataFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name)
				};

				await WebView2.EnsureCoreWebView2Async();
			}
		}

		public async void Navigate(string url)
        {
            if (WebView2.CoreWebView2 is null) await InitializeWebView();
			Dispatcher.BeginInvoke(() => { WebView2.CoreWebView2?.Navigate(url); });
		}
	}
}