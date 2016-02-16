using System;
using System.Collections.Generic;
using System.Globalization;
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
        public CultureInfo SourceLanguage { get; set; }
        public CultureInfo TargetLanguage { get; set; }
        public char Separator { get; set; }
        public string WorksheetName { get; set; }
        public Uri Uri { get; set; }

        public string GetExcelRangeAddress()
        {
            return string.IsNullOrEmpty(ApprovedColumn) 
                ? $"{SourceColumn.ToUpper()}:{TargetColumn.ToUpper()}" 
                : $"{SourceColumn.ToUpper()}:{ApprovedColumn.ToUpper()}";
        }
    }
}
