using System.Windows;
using System.Windows.Controls;

namespace MicrosoftTranslatorProvider.Controls
{
	public class PasswordBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached(
			"WatermarkText",
			typeof(string),
			typeof(PasswordBoxWatermarkHelper),
			new PropertyMetadata("Watermark", OnWatermarkTextChanged));

		public static readonly DependencyProperty IsWatermarkVisibleProperty = DependencyProperty.RegisterAttached(
			"IsWatermarkVisible",
			typeof(bool),
			typeof(PasswordBoxWatermarkHelper));

		public static string GetWatermarkText(DependencyObject control)
			=> (string)control.GetValue(WatermarkTextProperty);

		public static bool GetIsWatermarkVisible(DependencyObject control)
			=> (bool)control.GetValue(IsWatermarkVisibleProperty);

		public static void SetWatermarkText(DependencyObject control, string value)
			=> control.SetValue(WatermarkTextProperty, value);

		public static void SetIsWatermarkVisible(DependencyObject control, bool value)
			=> control.SetValue(IsWatermarkVisibleProperty, value);

		private static void OnWatermarkTextChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
		{
			var passwordBox = control as PasswordBox;
			SetIsWatermarkVisible(passwordBox, !string.IsNullOrEmpty(passwordBox.Password));
			passwordBox.LostFocus += OnControlLostFocus;
			passwordBox.GotFocus += OnControlGotFocus;
			passwordBox.Loaded -= TextChanged;
			passwordBox.Loaded += TextChanged;

			if (passwordBox.IsLoaded)
			{
				TextChanged(passwordBox, null);
			}
		}

		private static void TextChanged(object sender, RoutedEventArgs e)
		{
			if (sender is PasswordBox passwordBox)
			{
				SetIsWatermarkVisible(passwordBox, string.IsNullOrEmpty(passwordBox.Password));
			}
		}

		private static void OnControlGotFocus(object sender, RoutedEventArgs e)
		{
			SetIsWatermarkVisible(sender as PasswordBox, false);
		}

		private static void OnControlLostFocus(object sender, RoutedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;
			SetIsWatermarkVisible(passwordBox, string.IsNullOrEmpty(passwordBox.Password));
		}
	}
}