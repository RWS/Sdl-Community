using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Interface;
using Sdl.Community.GSVersionFetch.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Sdl.Community.GSVersionFetch.View
{
    /// <summary>
    /// Interaction logic for ProjectWizard.xaml
    /// </summary>
    public partial class ProjectWizard : IDisposable
    {
        private readonly ProjectWizardViewModel _model;

        public ProjectWizard(ObservableCollection<IProgressHeaderItem> pages)
        {
            InitializeComponent();

            UpdatePageIndexes(pages);
            _model = new ProjectWizardViewModel(this, pages);
            _model.SelectedPageChanged += Model_SelectedPageChanged;
            _model.RequestClose += ProjectWizardViewModel_RequestClose;

            DataContext = _model;
        }

        public void Dispose()
        {
            if (_model != null)
            {
                _model.RequestClose -= ProjectWizardViewModel_RequestClose;
                _model.Dispose();
            }
        }

        private static void UpdatePageIndexes(IReadOnlyList<IProgressHeaderItem> pages)
        {
            for (var i = 0; i < pages.Count; i++)
            {
                pages[i].PageIndex = i + 1;
                pages[i].TotalPages = pages.Count;
            }
        }

        private void Model_SelectedPageChanged(object sender, SelectedPageEventArgs e)
        {
            if (_model.CurrentPagePosition == e.PagePosition)
            {
                return;
            }

            if (!_model.CanMoveToPage(e.PagePosition, out var message))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, _model.WindowTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }

                return;
            }

            _model?.SetCurrentPage(e.PagePosition);
        }

        private void ProjectWizardViewModel_RequestClose(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}