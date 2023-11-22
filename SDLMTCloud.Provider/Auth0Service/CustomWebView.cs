using System.Windows;
using System.Windows.Input;
using Auth0Service.Commands;
using Microsoft.Web.WebView2.Wpf;

namespace Auth0Service
{
	public class CustomWebView : WebView2
	{
		public static readonly DependencyProperty DeleteAllCookiesCommandDependencyProperty =
			DependencyProperty.Register(
				name: nameof(DeleteAllCookiesCommand),
				propertyType: typeof(ICommand),
				ownerType: typeof(CustomWebView),
				typeMetadata: null);

		public ICommand DeleteAllCookiesCommand
		{
			get
			{
				return (ICommand)GetValue(DeleteAllCookiesCommandDependencyProperty);
			}
			set
			{
				SetValue(DeleteAllCookiesCommandDependencyProperty, value);
			}
		}

		public void Initialize(object sender, DependencyPropertyChangedEventArgs e)
		{
			DeleteAllCookiesCommand = new CommandHandler(DeleteAllCookies, true);
		}

		private void DeleteAllCookies()
		{
			CoreWebView2.CookieManager.DeleteAllCookies();
		}
	}
}