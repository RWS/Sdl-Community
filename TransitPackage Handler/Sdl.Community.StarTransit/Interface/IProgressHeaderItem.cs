﻿namespace Sdl.Community.StarTransit.Interface
{
	public interface IProgressHeaderItem
	{
		string DisplayName { get; set; }
		string Tooltip { get; set; }

		bool IsLastPage { get; }

		bool IsFirstPage { get; }

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

		bool CanCancel { get; set; }

		bool CanFinish { get; set; }

		bool OnChangePage(int position, out string message);

		void Dispose();
	}
}
