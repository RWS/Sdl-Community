using System;

namespace Sdl.Community.InvoiceAndQuotes.Projects
{
    public class ProjectProperty
    {
        private decimal _linesByCharacters;
        private decimal _linesByKeyStrokes;
        public StandardType StandardType { get; set; }
        public string Type { get; set; }
        public string PathInFile { get; set; }
        public decimal Rate { get; set; }
        public decimal Words { get; set; }
        public decimal ValueByWords { get; set; }
        public decimal Characters { get; set; }
        public decimal LinesByCharacters
        {
            get { return Math.Round(_linesByCharacters, 1); }
            set { _linesByCharacters = Math.Round(value, 1); }
        }

        public decimal ValueByLbC { get; set; }
        public decimal LinesByKeyStrokes
        {
            get { return Math.Round(_linesByKeyStrokes, 1); }
            set { _linesByKeyStrokes = Math.Round(value, 1); }
        }
        public decimal ValueByLbK { get; set; }
    }
}