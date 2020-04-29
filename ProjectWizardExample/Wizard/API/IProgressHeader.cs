using System;
using System.Collections.ObjectModel;
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

		void VisitNextPage();

		void VisitPreviousPage();

		void SetCurrentPage(int index);		

		void VisitSelectedPage(IProgressHeaderItem item);

		bool CanVisitPage(int position, out string message);

		ICommand SelectedPageCommand { get; }

		event EventHandler<SelectedPageEventArgs> SelectedPageChanged;
	}
}
