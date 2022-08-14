using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.MTCloud.Provider.UiHelpers.Watermark
{
	public class PasswordBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(PasswordBoxWatermarkHelper), new PropertyMetadata("Watermark", OnWatermarkTextChanged));

		public static readonly DependencyProperty WatermarkIsVisibleProperty = DependencyProperty.RegisterAttached("WatermarkIsVisible", typeof(bool), typeof(PasswordBoxWatermarkHelper));

		public static string GetWatermarkText(DependencyObject control)
			=> (string)control.GetValue(WatermarkTextProperty);

		public static bool GetWatermarkIsVisible(DependencyObject control)
			=> (bool)control.GetValue(WatermarkIsVisibleProperty);

		public static void SetWatermarkText(DependencyObject control, string value)
			=> control.SetValue(WatermarkTextProperty, value);

		public static void SetWatermarkIsVisible(DependencyObject control, bool value)
			=> control.SetValue(WatermarkIsVisibleProperty, value);

		private static void OnControlGotFocus(object sender, RoutedEventArgs e)
		{
			SetWatermarkIsVisible(sender as PasswordBox, false);
		}

		private static void OnControlLostFocus(object sender, RoutedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;
			if (passwordBox.Password.Length == 0)
			{
				SetWatermarkIsVisible(passwordBox, true);
			}
		}

		private static void OnWatermarkTextChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
		{
			var passwordBox = control as PasswordBox;
			SetWatermarkIsVisible(passwordBox, true);
			if (!string.IsNullOrEmpty(passwordBox.Password))
			{
				passwordBox.SetValue(WatermarkIsVisibleProperty, false);
				SetWatermarkIsVisible(passwordBox, false);
			}

			if (passwordBox is not null)
			{
				passwordBox.LostFocus += OnControlLostFocus;
				passwordBox.GotFocus += OnControlGotFocus;
			}
		}
	}
}