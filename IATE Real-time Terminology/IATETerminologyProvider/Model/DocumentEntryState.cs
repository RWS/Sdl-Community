using System.Collections.Generic;
using TradosStudio.API.TranslationResources.Terminology.Entries;


namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class DocumentEntryState
	{
		public string DocumentId { get; set; }
		public IEnumerable<Entry> Entries { get; set; }
		public Entry SelectedEntry { get; set; }
	}
}
