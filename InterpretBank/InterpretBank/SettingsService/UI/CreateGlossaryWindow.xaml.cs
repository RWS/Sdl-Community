using System.Windows;

namespace InterpretBank.SettingsService.UI
{
    /// <summary>
    /// Interaction logic for CreateGlossaryWindow.xaml
    /// </summary>
    public partial class CreateGlossaryWindow : Window
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(CreateGlossaryWindow), new PropertyMetadata(default(string)));

        public CreateGlossaryWindow()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}