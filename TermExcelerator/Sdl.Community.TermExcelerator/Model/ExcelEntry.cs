using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Model
{
	public class ExcelEntry : Entry
	{
		public IList<EntryField> Fields { get; set; }
		public int Id { get; set; }
		public bool IsDirty { get; set; }
		public IList<EntryLanguage> Languages { get; set; }
		public string SearchText { get; set; }
		public IList<EntryTransaction> Transactions { get; set; }
	}
}