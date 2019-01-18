using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.TMOptimizer.Control
{
	/// <summary>
	/// Interaction logic for OutputWizardPageControl.xaml
	/// </summary>
	public partial class OutputWizardPageControl : UserControl
    {
        public OutputWizardPageControl()
        {
            InitializeComponent();
        }

        private void BrowseNewTM_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectNewOutputTranslationMemory();
        }

        private void BrowseExistingTM_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectExistingOutputTranslationMemory();
        }

        public TMCleanerViewModel ViewModel
        {
            get
            {
                return (TMCleanerViewModel)DataContext;
            }
        }

        public bool Next()
        {
            return ViewModel.StartProcessing();
        }

        public bool Previous()
        {
            return true;
        }

        public void Help()
        {
           
        }

        public void Finish()
        {

        }

        public void Cancel()
        {

        }
    }
}