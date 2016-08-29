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
    /// Interaction logic for InputWizardPageControl.xaml
    /// </summary>
    public partial class InputWizardPageControl : UserControl
    {
        public InputWizardPageControl()
        {
            InitializeComponent();

            UpdateEnabled();
        }

        private void BrowseTM_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectExistingTranslationMemory();
        }

        private void AddTMX_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddTmxInputFile();
        }

        private void RemoveTMX_Click(object sender, RoutedEventArgs e)
        {
            List<InputTmxFile> selectedTmxFiles = new List<InputTmxFile>(_inputTmxFilesDataGrid.SelectedItems.Cast<InputTmxFile>());
            foreach (InputTmxFile f in selectedTmxFiles)
            {
                ViewModel.InputTmxFiles.Remove(f);
            }
        }

        public TMCleanerViewModel ViewModel
        {
            get
            {
                return (TMCleanerViewModel)DataContext;
            }
        }

        private void UpdateEnabled()
        {
            _removeButton.IsEnabled = _inputTmxFilesDataGrid.SelectedItems.Count > 0;
        }

        private void _inputTmxFilesDataGrid_SelectedCellsChanged_1(object sender, SelectedCellsChangedEventArgs e)
        {
            UpdateEnabled();
        }

        public bool Next()
        {
            return ViewModel.ValidateInput();
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
