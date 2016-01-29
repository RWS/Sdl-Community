using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ExcelTerminology.Model
{
    public class ExcelTerm
    {
        public string Source { get; set; }

        public CultureInfo SourceCulture { get; set; }
        public string Target { get; set; }
        public CultureInfo TargetCulture { get; set; }
        public string Approved { get; set; }
    }
}
