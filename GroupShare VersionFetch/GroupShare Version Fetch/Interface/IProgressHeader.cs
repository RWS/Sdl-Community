using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Sdl.Community.GSVersionFetch.Helpers;

namespace Sdl.Community.GSVersionFetch.Interface
{
	public interface IProgressHeader
	{
		ObservableCollection<IProgressHeaderItem> Pages { get; set; }

		IProgressHeaderItem CurrentPage { get; set; }

		string CompletedProgressStepsMessage { get; }

		void CalculateProgressHeaderItemsSize(double actualWidth);

		void UpdateCurrentPageState(bool isValid, bool isComplete);

		void MoveToNextPage();

		void MoveToPreviousPage();

		void SetCurrentPage(int index);

		void MoveToSelectedPage(IProgressHeaderItem item);

		bool CanMoveToPage(int position, out string message);

		ICommand SelectedPageCommand { get; }

		event EventHandler<SelectedPageEventArgs> SelectedPageChanged;
	}
}
