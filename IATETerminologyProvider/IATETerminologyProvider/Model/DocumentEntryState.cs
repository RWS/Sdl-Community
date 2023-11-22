using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class DocumentEntryState
	{
		public string DocumentId { get; set; }
		public IEnumerable<Entry> Entries { get; set; }
		public Entry SelectedEntry { get; set; }
	}
}
