using System.Collections.Generic;
using System.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Model
{
	public class ExcelEntryLanguage: IExcelEntryLanguage, IEntryLanguage
    {
        public bool IsSource { get; set; }
        public string Name { get; set; }
        public CultureInfo Locale { get; set; }
        public IEntry ParentEntry { get; set; }
        public IList<IEntryField> Fields { get; set; }
        public IList<IEntryTerm> Terms { get; set; }
    }
}