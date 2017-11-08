using System;
using System.ComponentModel;
using System.Windows;

namespace Sdl.Community.XmlReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XmlFileViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _viewModel = new XmlFileViewModel(null);
            base.DataContext = _viewModel;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _viewModel.ResetLists();
        }

        // Allow to drag and drop into the treeView only files
        public void treeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
        }

        public void treeView_Drop(object sender, DragEventArgs e)
        {
            string[] xmlFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            for (int i = 0; i < xmlFilePaths.Length; i++)
            {
                _viewModel.AddFile(xmlFilePaths[i]);
            }
        }

        public void buttonCleanAll_Click(object sender, EventArgs e)
        {
            _viewModel.ResetLists();
        }
    }
}
