using Sdl.Community.GSVersionFetch.Interface;
using System;

namespace Sdl.Community.GSVersionFetch.Helpers
{
    public class SelectedPageEventArgs : EventArgs
    {
        public int PagePosition { get; set; }
        public IProgressHeaderItem ProgressHeaderItem { get; set; }
    }
}