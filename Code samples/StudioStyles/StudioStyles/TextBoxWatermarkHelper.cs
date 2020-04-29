using System.Windows;
using System.Windows.Controls;

namespace StudioStyles
{
	public class TextBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty IsWatermarkVisibleProperty = DependencyProperty.RegisterAttached(
			"IsWatermarkVisible", typeof(bool), typeof(TextBoxWatermarkHelper));

		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached(
			"WatermarkText", typeof(string), typeof(TextBoxWatermarkHelper),
			new PropertyMetadata("Watermark", OnWatermarkTextChanged));

		public static string GetWatermarkText(TextBox control)
		{
			return (string) control.GetValue(WatermarkTextProperty);
		}

		public static bool GetIsWatermarkVisible(TextBox control)
		{
			return (bool) control.GetValue(IsWatermarkVisibleProperty);
		}

		public static void SetWatermarkText(TextBox control, string text)
		{
			control.SetValue(WatermarkTextProperty, text);
		}

		private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextBox control = d as TextBox;
			if (control == null) return;

			control.SetValue(IsWatermarkVisibleProperty, true);
			control.LostFocus += OnControlLostFocus;
			control.GotFocus += OnControlGotFocus;
		}

		private static void OnControlGotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).SetValue(IsWatermarkVisibleProperty, false);
		}

		private static void OnControlLostFocus(object sender, RoutedEventArgs e)
		{
			TextBox control = sender as TextBox;
			if (control != null)
			{
				if (string.IsNullOrEmpty(control.Text))
					control.SetValue(IsWatermarkVisibleProperty, true);
			}
		}
	}
}

