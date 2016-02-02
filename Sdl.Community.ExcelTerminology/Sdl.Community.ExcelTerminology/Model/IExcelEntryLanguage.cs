using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Model
{
    public interface IExcelEntryLanguage
    {
        bool IsSource { get; set; }
    }
}
