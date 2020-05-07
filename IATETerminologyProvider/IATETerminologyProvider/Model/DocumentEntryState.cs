using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class DocumentEntryState
	{
		public string DocumentId { get; set; }
		public IEnumerable<IEntry> Entries { get; set; }
		public IEntry SelectedEntry { get; set; }
	}
}
