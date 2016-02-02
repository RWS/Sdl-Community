using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Model
{
    public class ExcelEntry: IExcelEntry
    {
        public int Id { get; set; }
        public IList<IEntryField> Fields { get; set; }
        public IList<IEntryTransaction> Transactions { get; set; }
        public IList<IEntryLanguage> Languages { get; set; }
        public string SearchText { get; set; }
    }
}
