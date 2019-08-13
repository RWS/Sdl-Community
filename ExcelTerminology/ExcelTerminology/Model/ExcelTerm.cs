using System.Globalization;

namespace ExcelTerminology.Model
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