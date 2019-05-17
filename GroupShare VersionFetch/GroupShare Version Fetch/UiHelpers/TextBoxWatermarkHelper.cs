using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Sdl.Community.GSVersionFetch.Helpers;

namespace Sdl.Community.GSVersionFetch.UiHelpers
{
	public class TextBoxWatermarkHelper : DependencyObject
	{
		public static readonly DependencyProperty IsWatermarkVisibleProperty = DependencyProperty.RegisterAttached(
			"IsWatermarkVisible", typeof(bool), typeof(TextBoxWatermarkHelper));

		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached(
			"WatermarkText", typeof(string), typeof(TextBoxWatermarkHelper),
			new PropertyMetadata("Watermark", OnWatermarkTextChanged));

		public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached(
			"IsClearTextButtonBehaviorEnabled", typeof(bool), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));

		public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached(
			"ButtonCommand", typeof(ICommand), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));

		public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached(
			"ButtonCommandParameter", typeof(object), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(null));

		private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textbox = d as TextBox;
			if (textbox != null)
			{
				// only one loaded event
				textbox.Loaded -= TextChanged;
				textbox.Loaded += TextChanged;
				if (textbox.IsLoaded)
				{
					TextChanged(textbox, new RoutedEventArgs());
				}
			}
			var passbox = d as PasswordBox;
			if (passbox != null)
			{
				// only one loaded event
				passbox.Loaded -= PasswordChanged;
				passbox.Loaded += PasswordChanged;
				if (passbox.IsLoaded)
				{
					PasswordChanged(passbox, new RoutedEventArgs());
				}
			}
		}

		private static void IsClearTextButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var button = d as Button;
			if (e.OldValue != e.NewValue && button != null)
			{
				button.Click -= ButtonClicked;
				if ((bool)e.NewValue)
				{
					button.Click += ButtonClicked;
				}
			}
		}
		public static void SetButtonCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(ButtonCommandProperty, value);
		}

		public static void ButtonClicked(object sender, RoutedEventArgs e)
		{
			var button = (Button) sender;

			var parent = button.GetAncestors().FirstOrDefault(a => a is TextBox || a is PasswordBox || a is ComboBox);

			var command = GetButtonCommand(parent);
			var commandParameter = GetButtonCommandParameter(parent) ?? parent;
			if (command != null && command.CanExecute(commandParameter))
			{
				command.Execute(commandParameter);
			}
			if (parent is TextBox)
			{
				((TextBox) parent).Clear();
				((TextBox)parent).GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
			}
			else
			{
				(parent as PasswordBox)?.Clear();
			}
		}

		public static ICommand GetButtonCommand(DependencyObject d)
		{
			return (ICommand)d.GetValue(ButtonCommandProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(ButtonBase))]
		public static void SetIsClearTextButtonBehaviorEnabled(Button obj, bool value)
		{
			obj.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
		}
		private static void PasswordChanged(object sender, RoutedEventArgs e)
		{
			SetTextLength(sender as PasswordBox, passwordBox => passwordBox.Password.Length);
		}

		public static object GetButtonCommandParameter(DependencyObject d)
		{
			return d.GetValue(ButtonCommandParameterProperty);
		}

		private static void TextChanged(object sender, RoutedEventArgs e)
		{
			SetTextLength(sender as TextBox, textBox => textBox.Text.Length);
		}
		private static void SetTextLength<TDependencyObject>(TDependencyObject sender, Func<TDependencyObject, int> funcTextLength) where TDependencyObject : DependencyObject
		{
			if (sender != null)
			{
				var value = funcTextLength(sender);
			}
		}
		public static string GetWatermarkText(DependencyObject  control)
		{
			return (string)control.GetValue(WatermarkTextProperty);
		}

		public static bool GetIsWatermarkVisible(DependencyObject control)
		{
			return (bool)control.GetValue(IsWatermarkVisibleProperty);
		}

		public static void SetWatermarkText(DependencyObject control, string text)
		{
			control.SetValue(WatermarkTextProperty, text);
		}

		private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textBox = d as TextBox;
			var passwordBox = d as PasswordBox;
			d?.SetValue(IsWatermarkVisibleProperty, true);

			if (textBox!=null)
			{
				textBox.LostFocus += OnControlLostFocus;
				textBox.GotFocus += OnControlGotFocus;
			}
			if (passwordBox != null)
			{
				passwordBox.LostFocus += OnControlLostFocus;
				passwordBox.GotFocus += OnControlGotFocus;
			}
		}

		private static void OnControlGotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox)?.SetValue(IsWatermarkVisibleProperty, false);
			(sender as PasswordBox)?.SetValue(IsWatermarkVisibleProperty, false);
		}

		private static void OnControlLostFocus(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox control)
			{
				if (string.IsNullOrEmpty(control.Text))
				{
					control.SetValue(IsWatermarkVisibleProperty, true);
				}
			}
			if (sender is PasswordBox passwordControl)
			{
				if (string.IsNullOrEmpty(passwordControl.Password))
				{
					passwordControl.SetValue(IsWatermarkVisibleProperty, true);
				}
			}
		}
	}
}
