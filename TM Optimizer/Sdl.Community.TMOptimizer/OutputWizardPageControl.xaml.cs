using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.TMOptimizer
{
	/// <summary>
	/// Interaction logic for OutputWizardPageControl.xaml
	/// </summary>
	public partial class OutputWizardPageControl : UserControl, IWizardPageControl
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
            HelpManager.ShowHelp();
        }

        public void Finish()
        {

        }

        public void Cancel()
        {

        }
    }
}