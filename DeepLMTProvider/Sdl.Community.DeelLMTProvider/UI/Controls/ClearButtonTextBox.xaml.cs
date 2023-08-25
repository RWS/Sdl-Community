using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.UI.Controls
{
    /// <summary>
    /// Interaction logic for ClearButtonTextBox.xaml
    /// </summary>
    public partial class ClearButtonTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ClearButtonTextBox), new PropertyMetadata(default(string)));

        public ClearButtonTextBox()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private void ClearFilter()
        {
            Filter_TextBox.Clear();
            ClearFilter_Button.Visibility = Visibility.Collapsed;
        }

        private void ClearFilter_Button_Click(object sender, RoutedEventArgs e)
        {
            ClearFilter();
        }

        private void Filter_TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                ClearFilter();
        }

        private void Filter_TextBox_OnTextChanged(object sender, TextChangedEventArgs e) => ClearFilter_Button.Visibility = !string.IsNullOrWhiteSpace(Filter_TextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
    }
}