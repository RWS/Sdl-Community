using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.MTEdge.Provider.UiHelpers
{
	public class TextBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached(
			"ButtonCommandParameter", typeof(object), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(null));

		public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached(
			"ButtonCommand", typeof(ICommand), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));

		public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached(
			"IsClearTextButtonBehaviorEnabled", typeof(bool), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));

		public static readonly DependencyProperty IsWatermarkVisibleProperty = DependencyProperty.RegisterAttached(
									"IsWatermarkVisible", typeof(bool), typeof(TextBoxWatermarkHelper));

		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached(
			"WatermarkText", typeof(string), typeof(TextBoxWatermarkHelper), new PropertyMetadata("Watermark"));

		public static void ButtonClicked(object sender, RoutedEventArgs e)
		{
			var button = (Button)sender;

			var parent = button.GetAncestors().FirstOrDefault(a => a is TextBox || a is PasswordBox || a is ComboBox);

			var command = GetButtonCommand(parent);
			var commandParameter = GetButtonCommandParameter(parent) ?? parent;
			if (command != null && command.CanExecute(commandParameter))
			{
				command.Execute(commandParameter);
			}
		}

		public static ICommand GetButtonCommand(DependencyObject d)
		{
			return (ICommand)d.GetValue(ButtonCommandProperty);
		}

		public static object GetButtonCommandParameter(DependencyObject d)
		{
			return d.GetValue(ButtonCommandParameterProperty);
		}

		public static bool GetIsWatermarkVisible(DependencyObject control)
		{
			return (bool)control.GetValue(IsWatermarkVisibleProperty);
		}

		public static string GetWatermarkText(DependencyObject control)
		{
			return (string)control.GetValue(WatermarkTextProperty);
		}

		public static void SetButtonCommand(DependencyObject d, object value)
		{
			d.SetValue(ButtonCommandProperty, value);
		}

		public static void SetButtonCommandParameter(DependencyObject d, object value)
		{
			d.SetValue(ButtonCommandParameterProperty, value);
		}

		public static void SetIsClearTextButtonBehaviorEnabled(Button obj, bool value)
		{
			obj.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
		}

		public static void SetWatermarkText(DependencyObject control, string text)
		{
			control.SetValue(WatermarkTextProperty, text);
		}

		private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textbox = d as TextBox;
			if (textbox == null) return;
			// only one loaded event
			textbox.Loaded -= TextChanged;
			textbox.Loaded += TextChanged;
			if (textbox.IsLoaded)
			{
				TextChanged(textbox, new RoutedEventArgs());
			}
		}

		private static void IsClearTextButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var button = d as Button;
			if (e.OldValue == e.NewValue || button == null) return;
			button.Click -= ButtonClicked;
			if ((bool)e.NewValue)
			{
				button.Click += ButtonClicked;
			}
		}



		private static void SetTextLength<TDependencyObject>(TDependencyObject sender, Func<TDependencyObject, int> funcTextLength) where TDependencyObject : DependencyObject
		{
			if (sender == null) return;
			//var value = funcTextLength(sender);
			var textBox = sender as TextBox;
			var textBoxText = textBox?.Text;
			if (!string.IsNullOrEmpty(textBoxText))
			{
				textBox.SetValue(IsWatermarkVisibleProperty, false);
			}
		}

		private static void TextChanged(object sender, RoutedEventArgs e)
		{
			SetTextLength(sender as TextBox, textBox => textBox.Text.Length);
		}
	}

}