using System;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.XLIFF.Manager.Controls
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
			if (string.IsNullOrEmpty(e.NewValue.ToString()))
			{
				return;
			}

			if (!(o is WebBrowser browser))
			{
				return;
			}

			Uri uri = null;

			if (e.NewValue is string)
			{
				var uriString = e.NewValue as string;
				uri = string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString);
			}
			else if (e.NewValue is Uri uri1)
			{
				uri = uri1;
			}
			
			browser.Source = uri;
		}
	}
}
