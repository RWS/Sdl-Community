using System;
using Sdl.Community.GSVersionFetch.Interface;

namespace Sdl.Community.GSVersionFetch.Helpers
{
	public class SelectedPageEventArgs : EventArgs
	{
		public IProgressHeaderItem ProgressHeaderItem { get; set; }

		public int PagePosition { get; set; }
	}
}
