using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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

		public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
		public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextBoxWatermarkHelper), new UIPropertyMetadata(0));
		public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty ButtonContentTemplateProperty = DependencyProperty.RegisterAttached("ButtonContentTemplate", typeof(DataTemplate), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata((DataTemplate)null));
		public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.RegisterAttached("ButtonTemplate", typeof(ControlTemplate), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsClearTextButtonBehaviorEnabled", typeof(bool), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));
		public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached("ButtonCommand", typeof(ICommand), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));
		public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached("ButtonCommandParameter", typeof(object), typeof(TextBoxWatermarkHelper), new FrameworkPropertyMetadata(null));


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
			var combobox = d as ComboBox;
			if (combobox != null)
			{
				// only one loaded event
				combobox.Loaded -= ComboBoxLoaded;
				combobox.Loaded += ComboBoxLoaded;
				if (combobox.IsLoaded)
				{
					ComboBoxLoaded(combobox, new RoutedEventArgs());
				}
			}
		}
		static void ComboBoxLoaded(object sender, RoutedEventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox != null)
			{
				comboBox.SetValue(HasTextProperty, !string.IsNullOrWhiteSpace(comboBox.Text) || comboBox.SelectedItem != null);
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
				((TextBox)parent).SetValue(IsWatermarkVisibleProperty, true);
				((TextBox)parent).GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
			}
			else if (parent is PasswordBox)
			{
				((PasswordBox) parent).Clear();
				//((PasswordBox)parent).GetBindingExpression(PasswordBoxBindingBehavior.PasswordProperty)?.UpdateSource();
			}
			else if (parent is ComboBox)
			{
				if (((ComboBox) parent).IsEditable)
				{
					((ComboBox) parent).Text = string.Empty;
					((ComboBox) parent).GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
				}
				((ComboBox) parent).SelectedItem = null;
				((ComboBox) parent).GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
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
		public static bool GetClearTextButton(DependencyObject d)
		{
			return (bool)d.GetValue(ClearTextButtonProperty);
		}
		public static object GetButtonCommandParameter(DependencyObject d)
		{
			return (object)d.GetValue(ButtonCommandParameterProperty);
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
				sender.SetValue(TextLengthProperty, value);
				sender.SetValue(HasTextProperty, value >= 1);
			}
		}
		public static ControlTemplate GetButtonTemplate(DependencyObject d)
		{
			return (ControlTemplate)d.GetValue(ButtonTemplateProperty);
		}
		public static string GetWatermarkText(TextBox control)
		{
			return (string)control.GetValue(WatermarkTextProperty);
		}

		public static bool GetIsWatermarkVisible(TextBox control)
		{
			return (bool)control.GetValue(IsWatermarkVisibleProperty);
		}

		public static void SetWatermarkText(TextBox control, string text)
		{
			control.SetValue(WatermarkTextProperty, text);
		}

		private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as TextBox;
			if (control == null) return;

			control.SetValue(IsWatermarkVisibleProperty, true);
			control.LostFocus += OnControlLostFocus;
			control.GotFocus += OnControlGotFocus;
		}

		private static void OnControlGotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox)?.SetValue(IsWatermarkVisibleProperty, false);
		}

		private static void OnControlLostFocus(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox control)
			{
				if (string.IsNullOrEmpty(control.Text))
					control.SetValue(IsWatermarkVisibleProperty, true);
			}
		}
	}
}
