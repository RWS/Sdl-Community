using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoogleCloudTranslationProvider.UiHelpers
{
	public class TextBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached(
			"ButtonCommandParameter",
			typeof(object),
			typeof(TextBoxWatermarkHelper),
			new FrameworkPropertyMetadata(null));

		public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached(
			"ButtonCommand",
			typeof(ICommand),
			typeof(TextBoxWatermarkHelper),
			new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));

		public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached(
			"IsClearTextButtonBehaviorEnabled",
			typeof(bool),
			typeof(TextBoxWatermarkHelper),
			new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));

		public static readonly DependencyProperty IsWatermarkVisibleProperty = DependencyProperty.RegisterAttached(
			"IsWatermarkVisible",
			typeof(bool),
			typeof(TextBoxWatermarkHelper));

		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached(
			"WatermarkText",
			typeof(string),
			typeof(TextBoxWatermarkHelper),
			new PropertyMetadata("Watermark", OnWatermarkTextChanged));

		public static void ButtonClicked(object sender, RoutedEventArgs e)
		{
			var button = (Button)sender;
			var parent = button.GetAncestors().FirstOrDefault(a => a is TextBox || a is PasswordBox || a is ComboBox);
			var command = GetButtonCommand(parent);
			var commandParameter = GetButtonCommandParameter(parent) ?? parent;
			if (command != null
				&& command.CanExecute(commandParameter))
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

		public static bool GetIsWatermarkVisible(DependencyObject d)
		{
			return (bool)d.GetValue(IsWatermarkVisibleProperty);
		}

		public static string GetWatermarkText(DependencyObject d)
		{
			return (string)d.GetValue(WatermarkTextProperty);
		}

		public static void SetButtonCommand(DependencyObject d, object value)
		{
			d.SetValue(ButtonCommandProperty, value);
		}

		public static void SetButtonCommandParameter(DependencyObject d, object value)
		{
			d.SetValue(ButtonCommandParameterProperty, value);
		}

		public static void SetIsClearTextButtonBehaviorEnabled(Button button, bool value)
		{
			button.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
		}

		public static void SetWatermarkText(DependencyObject d, string text)
		{
			d.SetValue(WatermarkTextProperty, text);
		}

		private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is not TextBox textbox)
			{
				return;
			}

			textbox.Loaded -= TextChanged;
			textbox.Loaded += TextChanged;
			if (textbox.IsLoaded)
			{
				TextChanged(textbox, new RoutedEventArgs());
			}
		}

		private static void IsClearTextButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue == e.NewValue
				|| d is not Button button)
			{
				return;
			}

			button.Click -= ButtonClicked;
			if ((bool)e.NewValue)
			{
				button.Click += ButtonClicked;
			}
		}

		private static void OnControlGotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox)?.SetValue(IsWatermarkVisibleProperty, false);
		}

		private static void OnControlLostFocus(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox control
				&& string.IsNullOrEmpty(control.Text))
			{
				control.SetValue(IsWatermarkVisibleProperty, true);
			}
		}

		private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d?.SetValue(IsWatermarkVisibleProperty, true);
			if (d is not TextBox textBox)
			{
				return;
			}

			textBox.LostFocus += OnControlLostFocus;
			textBox.GotFocus += OnControlGotFocus;
		}

		private static void SetTextLength<TDependencyObject>(TDependencyObject sender, Func<TDependencyObject, int> funcTextLength) where TDependencyObject : DependencyObject
		{
			//var value = funcTextLength(sender);
			if (sender is not null
				&& sender is TextBox textBox
				&& !string.IsNullOrEmpty(textBox.Text))
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