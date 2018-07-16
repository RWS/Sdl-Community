using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
