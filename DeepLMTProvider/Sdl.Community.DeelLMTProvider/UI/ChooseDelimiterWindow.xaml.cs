using System.Windows;

namespace Sdl.Community.DeepLMTProvider.UI
{
    /// <summary>
    /// Interaction logic for ChooseDelimiterWindow.xaml
    /// </summary>
    public partial class ChooseDelimiterWindow
    {
        public static readonly DependencyProperty DelimiterProperty = DependencyProperty.Register(nameof(Delimiter), typeof(string), typeof(ChooseDelimiterWindow), new PropertyMetadata(default(string)));

        public ChooseDelimiterWindow()
        {
            InitializeComponent();
        }

        public string Delimiter
        {
            get => (string)GetValue(DelimiterProperty);
            set => SetValue(DelimiterProperty, value);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}