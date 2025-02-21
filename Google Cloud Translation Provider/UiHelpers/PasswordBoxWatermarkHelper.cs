using System.Windows;
using System.Windows.Controls;

namespace GoogleCloudTranslationProvider.UiHelpers
{
	public class PasswordBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(PasswordBoxWatermarkHelper), new PropertyMetadata("Watermark", OnWatermarkTextChanged));

		public static readonly DependencyProperty IsWatermarkVisibleProperty = DependencyProperty.RegisterAttached("IsWatermarkVisible", typeof(bool), typeof(PasswordBoxWatermarkHelper));

		public static string GetWatermarkText(DependencyObject control)
			=> (string)control.GetValue(WatermarkTextProperty);

		public static bool GetIsWatermarkVisible(DependencyObject control)
			=> (bool)control.GetValue(IsWatermarkVisibleProperty);

		public static void SetWatermarkText(DependencyObject control, string value)
			=> control.SetValue(WatermarkTextProperty, value);

		public static void SetIsWatermarkVisible(DependencyObject control, bool value)
			=> control.SetValue(IsWatermarkVisibleProperty, value);

		private static void OnControlGotFocus(object sender, RoutedEventArgs e)
		{
			SetIsWatermarkVisible(sender as PasswordBox, false);
		}

		private static void OnControlLostFocus(object sender, RoutedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;
			if (passwordBox.Password.Length == 0)
			{
				SetIsWatermarkVisible(passwordBox, true);
			}
		}

		private static void OnWatermarkTextChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
		{
			var passwordBox = control as PasswordBox;
			SetIsWatermarkVisible(passwordBox, true);
			if (!string.IsNullOrEmpty(passwordBox.Password))
			{
				passwordBox.SetValue(IsWatermarkVisibleProperty, false);
				SetIsWatermarkVisible(passwordBox, false);
			}

			if (passwordBox is not null)
			{
				passwordBox.LostFocus += OnControlLostFocus;
				passwordBox.GotFocus += OnControlGotFocus;
			}
		}
	}
}