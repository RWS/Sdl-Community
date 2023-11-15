using System.Collections.Generic;
using System.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Model
{
	public class ExcelEntryLanguage: EntryLanguage, IExcelEntryLanguage
    {
        public bool IsSource { get; set; }
        public string Name { get; set; }
        public CultureInfo Locale { get; set; }
        public Entry ParentEntry { get; set; }
        public IList<EntryField> Fields { get; set; }
        public IList<EntryTerm> Terms { get; set; }
    }
}