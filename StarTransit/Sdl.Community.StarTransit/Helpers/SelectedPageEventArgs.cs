using System;
using Sdl.Community.StarTransit.Interface;

namespace Sdl.Community.StarTransit.Helpers
{
	public class SelectedPageEventArgs : EventArgs
	{
		public IProgressHeaderItem ProgressHeaderItem { get; set; }

		public int PagePosition { get; set; }
	}
}
