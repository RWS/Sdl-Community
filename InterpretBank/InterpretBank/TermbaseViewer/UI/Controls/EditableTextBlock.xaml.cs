using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace InterpretBank.TermbaseViewer.UI.Controls
{
	/// <summary>
	/// Interaction logic for EditableTextBlock.xaml
	/// </summary>
	public partial class EditableTextBlock : UserControl
	{
		public static readonly DependencyProperty IsEditingProperty =
			DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false, OnIsEditingPropertyChanged));

		private static void OnIsEditingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (EditableTextBlock)d;
			control.SetEditing();
		}


		public static readonly DependencyProperty TextProperty =
					DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableTextBlock), new PropertyMetadata(""));

		public EditableTextBlock()
		{
			InitializeComponent();
		}

		public bool IsEditing
		{
			get { return (bool)GetValue(IsEditingProperty); }
			set
			{
				SetValue(IsEditingProperty, value);
			}
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		private void SetEditing()
		{
			if (IsEditing)
			{
				EditBox.Visibility = Visibility.Visible;
				TextBlock.Visibility = Visibility.Collapsed;
			}
			else
			{
				EditBox.Visibility = Visibility.Collapsed;
				TextBlock.Visibility = Visibility.Visible;
			}
		}

		private void EditBox_OnGotFocus(object sender, RoutedEventArgs e)
		{
			var hexColor = "#bed7e8";
			var color = (Color)ColorConverter.ConvertFromString(hexColor);
			EditBox.Background = new SolidColorBrush(color);
		}

		private void EditBox_OnLostFocus(object sender, RoutedEventArgs e)
		{
			EditBox.Background = new SolidColorBrush(Colors.Transparent);
		}
	}
}