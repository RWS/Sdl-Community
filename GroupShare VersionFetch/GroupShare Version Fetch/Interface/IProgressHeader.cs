using Sdl.Community.GSVersionFetch.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sdl.Community.GSVersionFetch.Interface
{
    public interface IProgressHeader
    {
        event EventHandler<SelectedPageEventArgs> SelectedPageChanged;

        string CompletedProgressStepsMessage { get; }
        IProgressHeaderItem CurrentPage { get; set; }
        ObservableCollection<IProgressHeaderItem> Pages { get; set; }
        ICommand SelectedPageCommand { get; }

        void CalculateProgressHeaderItemsSize(double actualWidth);

        bool CanMoveToPage(int position, out string message);

        void MoveToNextPage();

        void MoveToPreviousPage();

        void MoveToSelectedPage(IProgressHeaderItem item);

        void SetCurrentPage(int index);
    }
}