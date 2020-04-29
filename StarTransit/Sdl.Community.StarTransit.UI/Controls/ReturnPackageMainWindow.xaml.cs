using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Sdl.Community.StarTransit.Shared.Interfaces;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.ViewModels;

namespace Sdl.Community.StarTransit.UI.Controls
{
	/// <summary>
	/// Interaction logic for ReturnPackageMainWindow.xaml
	/// </summary>
	public partial class ReturnPackageMainWindow
    {
        private readonly ReturnFiles _returnPackageFiles;
        private readonly CellViewModel _cellViewModel;

        public ReturnPackageMainWindow(ReturnPackage returnPackage)
        {
            InitializeComponent();

            IMessageBoxService messageBoxService = new MessageBoxService();
            var returnFilesViewModel = new ReturnFilesViewModel(returnPackage, messageBoxService);
            if (returnFilesViewModel?.ProjectFiles == null)
            {
                Close();
                return;
            }

            _returnPackageFiles = new ReturnFiles(returnFilesViewModel);
            _cellViewModel = new CellViewModel();

            var returnPackageMainWindowViewModel = new ReturnPackageMainWindowViewModel(returnFilesViewModel, _cellViewModel, messageBoxService);
            DataContext = returnPackageMainWindowViewModel;
            if (returnPackageMainWindowViewModel.CloseAction == null)
            {
                returnPackageMainWindowViewModel.CloseAction = Close;
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReturnPackageFiles.IsSelected = true;
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var li = sender as ListViewItem;
            if (li == null)
            {
                return;
            }

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

        private void ReturnPackageMainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _cellViewModel?.ClearSelectedProjectsList();
        }
    }
}