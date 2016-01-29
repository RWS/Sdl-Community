using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ExcelTerminology.Model
{
    public class ProviderSettings
    {
        public string SourceColumn { get; set; }
        public string TargetColumn { get; set; }
        public string ApprovedColumn { get; set; }
        public bool HasHeader { get; set; }
        public string TermFilePath { get; set; }
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public string Separator { get; set; }
        public string WorksheetName { get; set; }

    }
}
