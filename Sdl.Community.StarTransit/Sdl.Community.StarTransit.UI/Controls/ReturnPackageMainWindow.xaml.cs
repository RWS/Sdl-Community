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
using MahApps.Metro.Controls;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.ViewModels;

namespace Sdl.Community.StarTransit.UI.Controls
{
    /// <summary>
    /// Interaction logic for ReturnPackageMainWindow.xaml
    /// </summary>
    public partial class ReturnPackageMainWindow : MetroWindow
    {
        private readonly ReturnFiles _returnPackageFiles;

        public ReturnPackageMainWindow(ReturnPackage returnPackage)
        {
            InitializeComponent();

            var returnFilesViewModel = new ReturnFilesViewModel(returnPackage);
            _returnPackageFiles = new ReturnFiles(returnFilesViewModel);

            var returnPackageMainWindowViewModel = new ReturnPackageMainVindowViewModel();
            DataContext = returnPackageMainWindowViewModel;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReturnPackageFiles.IsSelected = true;
        }
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            string tag = string.Empty;

            if (sender == null) return;
            var li = sender as ListViewItem;
            if (li == null) return;
            switch (li.Tag.ToString())
            {
                case "packageFiles":
                    tcc.Content = _returnPackageFiles;
                    break;

                default:
                    tcc.Content = _returnPackageFiles;

                    break;
            }

        }
    }
}
