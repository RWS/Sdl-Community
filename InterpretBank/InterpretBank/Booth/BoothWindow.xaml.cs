using InterpretBank.Booth.ViewModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace InterpretBank.Booth
{
    /// <summary>
    /// Interaction logic for BoothWindow.xaml
    /// </summary>
    public partial class BoothWindow : Window
    {
        public BoothWindow(BoothWindowViewModel boothWindowViewModel)
        {
            DataContext = boothWindowViewModel;
            InitializeComponent();

        }


        private void SettingsToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton toggleButton) return;
            if (toggleButton.IsChecked ?? false)
            {
                LastColumnWidth = ColumnOne.Width;
                ColumnOne.MinWidth = 0;
                ColumnOne.Width = GridLength.Auto;
                SettingsGroupBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                ColumnOne.Width = LastColumnWidth;
                ColumnOne.MinWidth = 200;
                SettingsGroupBox.Visibility = Visibility.Visible;
            }
        }

        private GridLength LastColumnWidth { get; set; }
    }
}