using InterpretBank.Booth.ViewModel;
using System.Windows;
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

        
    }
}