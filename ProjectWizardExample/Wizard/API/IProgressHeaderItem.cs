namespace ProjectWizardExample.Wizard.API
{
	public interface IProgressHeaderItem
	{
		string DisplayName { get; set; }

		bool IsLastPage { get; }

		bool IsFirstPage { get; }

		int PagePosition { get; set; }

		int PageIndex { get; set; }

		int TotalPages { get; set; }

		double ItemLineWidth { get; set; }

		double ItemTextWidth { get; set; }

		bool IsCurrentPage { get; set; }

		bool IsVisited { get; set; }

		bool NextIsVisited { get; set; }

		bool PreviousIsVisited { get; set; }

		bool IsValid { get; set; }		

		bool IsComplete { get; set; }

		/// <summary>
		/// Executed when a request is made to change the current page to the
		/// page position in the pages collection.
		/// </summary>
		/// <param name="position">The new page position in the pages collection.</param>
		/// <param name="message">An error message</param>
		/// <returns>Returns true is no error; false if an error occurs that pervents
		/// the user from moving away from the page (e.g. data validation error on the page)</returns>
		bool OnPageChange(int position, out string message);
	}
}
