using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        public bool IsReadOnly { get; set; }
        public Uri Uri { get; set; }


        public bool IsFileReady()
        {

            var result = true;
            try
            {
                using (new FileStream(TermFilePath, FileMode.OpenOrCreate))
                {
                }
            }
            catch (IOException)
            {
                result = false;
            }
            return result;
        }
    }
}
