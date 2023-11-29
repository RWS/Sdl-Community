using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LanguageWeaverProvider.Controls
{
	public class ToggleOption : Control
	{
		public static readonly DependencyProperty CheckBoxContentProperty = DependencyProperty.Register(
			"CheckBoxContent",
			typeof(object),
			typeof(ToggleOption));

		public static readonly DependencyProperty DescriptionTextProperty = DependencyProperty.Register(
			"DescriptionText",
			typeof(string),
			typeof(ToggleOption));

		public static readonly DependencyProperty TextBoxVisibleProperty = DependencyProperty.Register(
			"TextBoxVisible",
			typeof(bool),
			typeof(ToggleOption),
			new PropertyMetadata(true));

		public static readonly DependencyProperty TextBoxTextProperty = DependencyProperty.Register(
			"TextBoxText",
			typeof(string),
			typeof(ToggleOption),
			new PropertyMetadata("Watermark", OnWatermarkTextChanged));

		private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

		}

		public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register(
			"Checked",
			typeof(bool),
			typeof(ToggleOption),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register(
			"ButtonCommand",
			typeof(ICommand),
			typeof(ToggleOption));

		public static readonly DependencyProperty ButtonParameterProperty = DependencyProperty.Register(
			"ButtonParameter",
			typeof(string),
			typeof(ToggleOption));


		public object CheckBoxContent
		{
			get => GetValue(CheckBoxContentProperty);
			set => SetValue(CheckBoxContentProperty, value);
		}

		public string DescriptionText
		{
			get => (string)GetValue(DescriptionTextProperty);
			set => SetValue(DescriptionTextProperty, value);
		}

		public bool TextBoxVisible
		{
			get => (bool)GetValue(TextBoxVisibleProperty);
			set => SetValue(TextBoxVisibleProperty, value);
		}

		public string TextBoxText
		{
			get => (string)GetValue(TextBoxTextProperty);
			set => SetValue(TextBoxTextProperty, value);
		}

		public bool Checked
		{
			get => (bool)GetValue(CheckedProperty);
			set => SetValue(CheckedProperty, value);
		}

		public ICommand ButtonCommand
		{
			get => (ICommand)GetValue(ButtonCommandProperty);
			set => SetValue(ButtonCommandProperty, value);
		}

		public string ButtonParameter
		{
			get => (string)GetValue(ButtonParameterProperty);
			set => SetValue(DescriptionTextProperty, value);
		}
	}
}