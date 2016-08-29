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
using System.Windows.Shapes;

namespace Sdl.Community.TMOptimizer.Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _wizard.NextButtonContent = "_Next";
            _wizard.HelpButtonContent = "_Help";
            _wizard.CancelButtonContent = "C_ancel";
            _wizard.BackButtonContent = "_Previous";
            _wizard.FinishButtonContent = "_Finish";
        }

        public TMCleanerViewModel ViewModel
        {
            get
            {
                return (TMCleanerViewModel)DataContext;
            }
        }

        private void Wizard_Next(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            IWizardPageControl c = _wizard.CurrentPage.Content as IWizardPageControl;

            if (c != null)
            {
                e.Cancel = !c.Next();
            }
        }

        private void Wizard_Previous(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            IWizardPageControl c = _wizard.CurrentPage.Content as IWizardPageControl;

            if (c != null)
            {
                e.Cancel = !c.Previous();
            }
        }

        private void Wizard_Cancel(object sender, RoutedEventArgs e)
        {
            IWizardPageControl c = _wizard.CurrentPage.Content as IWizardPageControl;

            if (c != null)
            {
                c.Cancel();
            }
        }

        private void Wizard_Finish(object sender, RoutedEventArgs e)
        {
            IWizardPageControl c = _wizard.CurrentPage.Content as IWizardPageControl;

            if (c != null)
            {
                c.Finish();
            }
        }

        private void Wizard_Help(object sender, RoutedEventArgs e)
        {
            HelpManager.ShowHelp();
        }
    }
}
