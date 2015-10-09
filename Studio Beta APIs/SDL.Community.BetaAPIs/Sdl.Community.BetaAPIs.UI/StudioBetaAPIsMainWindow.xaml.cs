using Sdl.Community.BetaAPIs.UI.ViewModel;
using System.Windows;

namespace Sdl.Community.BetaAPIs.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class StudioBetaAPIsMainWindow : Window
    {
        
        public StudioBetaAPIsMainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModelLocator().MainViewModel;
        }
    }
}
