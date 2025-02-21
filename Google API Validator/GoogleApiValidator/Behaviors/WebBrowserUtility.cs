using System.Windows;
using System.Windows.Controls;
using mshtml;

namespace Sdl.Community.GoogleApiValidator.Behaviors
{
	public static class WebBrowserUtility
	{
		public static readonly DependencyProperty HideScrollBarProperty =
			DependencyProperty.RegisterAttached("HideScrollBar",
			typeof(string),
			typeof(WebBrowserUtility),
			new UIPropertyMetadata(null, HideScrollBarPropertyChanged));

		public static string GetHideScrollBar(DependencyObject obj)
		{
			return (string)obj.GetValue(HideScrollBarProperty);
		}
		public static void SetHideScrollBar(DependencyObject obj, string value)
		{
			obj.SetValue(HideScrollBarProperty, value);
		}
		public static void HideScrollBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var browser = obj as WebBrowser;
			var str = args.NewValue as string;
			bool isHidden;
			if (str != null && bool.TryParse(str, out isHidden))
			{
				browser.HideScrollBar(isHidden);
			}
		}

		public static readonly DependencyProperty TopProperty =
			DependencyProperty.RegisterAttached("Top",
				typeof(int),
				typeof(WebBrowserUtility),
				new UIPropertyMetadata(0, TopPropertyChanged));

		public static int GetTop(DependencyObject obj)
		{
			return (int)obj.GetValue(TopProperty);
		}
		public static void SetTop(DependencyObject obj, int value)
		{
			obj.SetValue(HideScrollBarProperty, value);
		}
		public static void TopPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var browser = obj as WebBrowser;
			if (args.NewValue != null)
			{
				int top = (int)args.NewValue;

				browser.SetTop(top);
			}
		}
	}

	public static class WebBrowserExtension
	{
		public static void HideScrollBar(this WebBrowser browser, bool isHidden)
		{
			if (browser != null)
			{
				if (!(browser.Document is IHTMLDocument2 document))
				{
					// If too early
					browser.LoadCompleted += (o, e) => HideScrollBar(browser, isHidden);
					return;
				}

				var elementOverflow = $"document.body.style.overflow='{(isHidden ? "hidden" : "auto")}';";
				document.parentWindow.execScript(elementOverflow);
			}
		}
		public static void SetTop(this WebBrowser browser, int top)
		{
			if (browser != null)
			{
				if (!(browser.Document is IHTMLDocument2 document))
				{
					// If too early
					browser.LoadCompleted += (o, e) =>
					{
						SetTop(browser, top);
					};
					return;
				}

				document.body.style.top = top + "px";
			}
		}
	}
}
