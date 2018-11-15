using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Model
{
	public class EntryModel : IEntry
	{
		public string SearchText { get; set; }

		public int Id { get; set; }

		public IList<IEntryField> Fields { get; set; }

		public IList<IEntryTransaction> Transactions { get; set; }

		public IList<IEntryLanguage> Languages { get; set; }
	}
}