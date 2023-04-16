using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Sdl.Community.MTCloud.Provider.UiHelpers.Watermark
{

	// note: initially, I wanted to create two simple classes, one deriving from TextBox, and one from PasswordBox
	// but, PasswordBox is sealed
	public class WatermarkHandler
	{
		private static List<WatermarkControlHandler<TextBox>> _textBoxes = new List<WatermarkControlHandler<TextBox>>();
		private static List<WatermarkControlHandler<PasswordBox>> _passwordBoxes = new List<WatermarkControlHandler<PasswordBox>>();

		public static void Handle(TextBox tb)
		{
			_textBoxes.Add(new WatermarkControlHandler<TextBox>(tb));
			tb.Unloaded += Tb_Unloaded;
		}
		public static void Handle(PasswordBox pb)
		{
			_passwordBoxes.Add(new WatermarkControlHandler<PasswordBox>(pb));
			pb.Unloaded += Pb_Unloaded;
		}

		private static void Pb_Unloaded(object sender, System.Windows.RoutedEventArgs e)
		{
			var control = sender as PasswordBox;
			control.Unloaded -= Pb_Unloaded;
			_passwordBoxes.RemoveAll(pb => ReferenceEquals(control, pb));
		}

		private static void Tb_Unloaded(object sender, System.Windows.RoutedEventArgs e)
		{
			var control = sender as TextBox;
			control.Unloaded -= Tb_Unloaded;
			_textBoxes.RemoveAll(tb => ReferenceEquals(control, tb));
		}
	}

	public class WatermarkControlHandler<T> where T : Control
	{
		private T _control;
		private bool _hasFocus;

		public T Control => _control;

		public WatermarkControlHandler(T control)
		{
			_control = control;
			_control.GotFocus += _control_GotFocus;
			_control.LostFocus += _control_LostFocus;
			_control.Loaded += _control_Loaded;
			if (control is TextBox tb)
				tb.TextChanged += Tb_TextChanged;
			else if (control is PasswordBox pb)
				pb.PasswordChanged += Pb_PasswordChanged;
			else
				throw new Exception($"control not a textbox/passwordbox");
		}

		private void _control_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			UpdateWatermark();
		}

		private void Pb_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			UpdateWatermark();
		}

		private void Tb_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateWatermark();
		}

		private string Text()
		{
			if (_control is TextBox tb)
				return tb.Text;
			if (_control is PasswordBox pb)
				return pb.Password;

			throw new Exception($"control not a textbox/passwordbox");
		}
		private void UpdateWatermark()
		{
			var showWatermark = Text() == "" && !_hasFocus;
			if (_control is TextBox)
				_control.SetValue(TextBoxWatermarkHelper.IsWatermarkVisibleProperty, showWatermark);
			else if (_control is PasswordBox)
				_control.SetValue(PasswordBoxWatermarkHelper.IsWatermarkVisibleProperty, showWatermark);
		}

		private void _control_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			_hasFocus = false;
			UpdateWatermark();
		}

		private void _control_GotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			_hasFocus = true;
			UpdateWatermark();
		}

	}
}
