using Sdl.Community.XmlReader.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Sdl.Community.XmlReader.WPF
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
	        var multiSelect = new MultiSelectTreeView(_viewModel);
		   DataContext = _viewModel;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _viewModel.ResetLists();
        }

        public void treeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
        }

        public void treeView_Drop(object sender, DragEventArgs e)
        {
	        var xmlFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
	        if (xmlFilePaths != null)
	        {
		        foreach (var filePath in xmlFilePaths)
		        {
			        _viewModel.AddFile(filePath);
		        }
		        _viewModel.IsClearEnabled = true;
	        }
        }

        public void treeView_RemoveItems(object sender, EventArgs e)
        {
            List<TargetLanguageCodeViewModel> parentsToRemove = new List<TargetLanguageCodeViewModel>();
            foreach (var parentItem in _viewModel.XmlFiles)
            {
                if (parentItem.IsSelected)
                {
                    parentsToRemove.Add(parentItem);
                    continue;
                }

                List<AnalyzeFileViewModel> childrenToRemove = new List<AnalyzeFileViewModel>();
                foreach (var childItem in parentItem.Children)
                {
                    var child = childItem as AnalyzeFileViewModel;
                    if (parentItem.Children.Count == 1 && childItem.IsSelected)
                    {
                        parentsToRemove.Add(parentItem);
                    }
                    else if (child.IsSelected)
                    {
                        childrenToRemove.Add(child);
                    }
                }

                _viewModel.RemoveChildren(parentItem.TargetLanguageCode, childrenToRemove);
            }

            foreach (var parent in parentsToRemove)
            {
                _viewModel.RemoveParent(parent);
            }
        }

    }
}
