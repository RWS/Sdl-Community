using System.Collections.Generic;

namespace Sdl.Community.TargetWordCount.Models
{
	public class SerializableSettings
    {
        public string Culture { get; set; }

        public bool IncludeSpaces { get; set; }

        public List<InvoiceItem> InvoiceRates { get; set; }

        public bool ReportLockedSeperately { get; set; }

        public bool UseLineCount { get; set; }

        public bool UseSource { get; set; }

        public string CharactersPerLine { get; set; }
    }
}