using Sdl.Community.XmlReader.Data;
using System.Windows;

namespace Sdl.Community.XmlReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Populate languageCode list with mock dates
            LocalDatabase.MockLanguageCodes();

            var _codes = LocalDatabase.GetLanguageCodes();
            XmlFileViewModel viewModel = new XmlFileViewModel(_codes);
            base.DataContext = viewModel;
        }
    }
}
