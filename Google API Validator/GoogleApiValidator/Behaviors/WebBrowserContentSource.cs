using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.GoogleApiValidator.Behaviors
{
	public static class WebBrowserContentSource
	{
		public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
			"Html",
			typeof(string),
			typeof(WebBrowserContentSource),
			new FrameworkPropertyMetadata(OnHtmlChanged));

		[AttachedPropertyBrowsableForType(typeof(WebBrowser))]
		public static string GetHtml(WebBrowser d)
		{
			return (string)d.GetValue(HtmlProperty);
		}

		public static void SetHtml(WebBrowser d, string value)
		{
			d.SetValue(HtmlProperty, value);
		}

		static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is WebBrowser wb && !string.IsNullOrEmpty(e.NewValue.ToString()))
				wb.NavigateToString(e.NewValue as string);
		}
	}
}

