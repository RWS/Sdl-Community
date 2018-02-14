using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQA
{
    public class Entry
    {
        public string language { get; set; }
        public string file { get; set; }
        public string segment { get; set; }
        public string originalTranslation { get; set; }
        public List<Tuple<string, TextType>> revisedTranslation { get; set; }
        public string highlightedText { get; set; }
        public List<Tuple<string, TextType>> sourceContent { get; set; }
        public string category { get; set; }
        public string severity { get; set; }
        public string comment { get; set; }

        public Entry(string language, string file, string segment, string originalTranslation, List<Tuple<string, TextType>> revisedTranslation, List<Tuple<string, TextType>> sourceContent, string category, string severity, string comment, string highlightedText)
        {
            this.language = language;
            this.file = file;
            this.segment = segment;
            this.originalTranslation = originalTranslation;
            this.revisedTranslation = revisedTranslation;
            this.sourceContent = sourceContent;
            this.category = category;

            switch( severity )
            {
                case "Minor weight":
                    this.severity = "Minor Error";
                    break;
                case "Serious weight":
                    this.severity = "Serious Error";
                    break;
                default:
                    this.severity = severity;
                    break;
            }

            this.comment = comment;
            this.highlightedText = highlightedText;
        }

        public string[] getArray()
        {
            return new string[] { this.file, this.segment, "", this.originalTranslation, "", this.category, this.severity, this.comment };
        }
    }

    public enum TextType
    {
        Added,
        Deleted,
        Regular,
        Comment
    }
}
