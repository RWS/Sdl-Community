using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace Sdl.Community.IATETerminologyProvider.View
{
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : UserControl
	{
		private Task _initializationTask;

		public BrowserWindow()
		{
			InitializeComponent();
		}

		public Task InitializeWebView()
		{
			if (!Dispatcher.CheckAccess())
			{
				return Dispatcher.InvokeAsync(() => InitializeWebView()).Task.Unwrap();
			}

			// Cache the task so concurrent callers don't initialize WebView2 more than once.
			return _initializationTask ?? (_initializationTask = InitializeWebViewCoreAsync());
		}

		private async Task InitializeWebViewCoreAsync()
		{
			if (WebView2.CoreWebView2 != null)
			{
				return;
			}

			WebView2.CreationProperties = new CoreWebView2CreationProperties
			{
				UserDataFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name)
			};

			if (!IsLoaded)
			{
				await WaitUntilLoadedAsync();
			}

			await WebView2.EnsureCoreWebView2Async();
		}

		private Task WaitUntilLoadedAsync()
		{
			if (IsLoaded)
			{
				return Task.CompletedTask;
			}

			var completionSource = new TaskCompletionSource<bool>();
			RoutedEventHandler loadedHandler = null;
			loadedHandler = (_, _) =>
			{
				Loaded -= loadedHandler;
				completionSource.TrySetResult(true);
			};

			Loaded += loadedHandler;
			return completionSource.Task;
		}

		public async Task NavigateAsync(string url)
		{
			await InitializeWebView();

			if (!Dispatcher.CheckAccess())
			{
				await Dispatcher.InvokeAsync(() => WebView2.CoreWebView2.Navigate(url)).Task;
				return;
			}

			WebView2.CoreWebView2.Navigate(url);
		}
	}
}