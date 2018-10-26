using System;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
	public class BindingWebBrowserSource
	{
		public static readonly DependencyProperty BindableSourceProperty =
			DependencyProperty.RegisterAttached("BindableSource", typeof(string), 
				typeof(BindingWebBrowserSource), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

		public static string GetBindableSource(DependencyObject obj)
		{
			return (string)obj.GetValue(BindableSourceProperty);
		}

		public static void SetBindableSource(DependencyObject obj, string value)
		{
			obj.SetValue(BindableSourceProperty, value);
		}

		public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o is WebBrowser browser)
			{
				var uri = e.NewValue as string;
				browser.Source = !string.IsNullOrEmpty(uri) ? new Uri(uri) : null;
			}
		}
	}
}
