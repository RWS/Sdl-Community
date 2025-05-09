using System.Collections.Generic;

namespace GroupshareExcelAddIn.Models
{
    public class FilterParameter
    {
        public LanguagePair LanguagePair { get; set; }
        public List<string> ResourceTypes { get; set; }
    }
}