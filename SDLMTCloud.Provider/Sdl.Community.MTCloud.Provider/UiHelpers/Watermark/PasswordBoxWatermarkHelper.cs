using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.MTCloud.Provider.UiHelpers.Watermark
{
	public class PasswordBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(PasswordBoxWatermarkHelper), new PropertyMetadata("Watermark"));

		public static readonly DependencyProperty IsWatermarkVisibleProperty = DependencyProperty.RegisterAttached("IsWatermarkVisible", typeof(bool), typeof(PasswordBoxWatermarkHelper));

		public static string GetWatermarkText(DependencyObject control)
			=> (string)control.GetValue(WatermarkTextProperty);

		public static bool GetIsWatermarkVisible(DependencyObject control)
			=> (bool)control.GetValue(IsWatermarkVisibleProperty);

		public static void SetWatermarkText(DependencyObject control, string value)
			=> control.SetValue(WatermarkTextProperty, value);

		public static void SetIsWatermarkVisible(DependencyObject control, bool value)
			=> control.SetValue(IsWatermarkVisibleProperty, value);

	}
}