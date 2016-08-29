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
    /// Interaction logic for ProcessingWizardPageControl.xaml
    /// </summary>
    public partial class ProcessingWizardPageControl : UserControl
    {
        public ProcessingWizardPageControl()
        {
            InitializeComponent();
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
            return true;
        }

        public bool Previous()
        {
            if (ViewModel.IsProcessing)
            {
                MessageBox.Show(Application.Current.MainWindow, "You cannot go back while the wizard is processing.", "Processing", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
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

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenContainerFolder();
        }

        private void OpenTMInStudio_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenOutputTMInStudio();
        }
    }
}
