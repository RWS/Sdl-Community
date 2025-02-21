using System;
using SDLTM.Import.Interface;

namespace SDLTM.Import.Helpers
{

	public class SelectedPageEventArgs : EventArgs
	{
		public IProgressHeaderItem ProgressHeaderItem { get; set; }

		public int PagePosition { get; set; }
	}

}
