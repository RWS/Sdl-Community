using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using ProjectWizardExample.EventArgs;

namespace ProjectWizardExample.Wizard.API
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

		bool CanMoveToPage(int index, out string message);

		ICommand SelectedPageCommand { get; }

		event EventHandler<SelectedPageEventArgs> SelectedPageChanged;
	}
}
