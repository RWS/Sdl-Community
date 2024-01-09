using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InterpretBank.Controls
{
    /// <summary>
    /// Interaction logic for WatermarkTextBox2.xaml
    /// </summary>
    public partial class WatermarkTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text),
            typeof(string), typeof(WatermarkTextBox), new PropertyMetadata(default(string)));

        public WatermarkTextBox()
        {
            InitializeComponent();
            Watermark.Visibility = Visibility.Visible;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private void TextBox_OnIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Watermark.Visibility = Visibility.Collapsed;
            }
            else if (!(bool)e.NewValue && string.IsNullOrWhiteSpace(Text))
            {
                Watermark.Visibility = Visibility.Visible;
            }
        }
    }
}