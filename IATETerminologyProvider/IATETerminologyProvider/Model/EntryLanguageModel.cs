using System.Collections.Generic;
using System.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Model
{
	public class EntryLanguageModel : IEntryLanguage
	{
		public bool IsSource { get; set; }
		public IEntry ParentEntry { get; set; }
		public IList<IEntryField> Fields { get; set; }
		public IList<IEntryTerm> Terms { get; set; }
		public string Name { get; set; }
		public CultureInfo Locale { get; set; }
	}
}