using System.Xml.Serialization;

namespace InterpretBank.Model
{
    [XmlRoot("TermbaseSettings")]
    public class TermbaseSettings
    {
        //[XmlElement("Filter")]
        //public int Filter { get; set; }

        //[XmlElement("FilterHighlight")]
        //public bool FilterHighlight { get; set; }

        //[XmlElement("IsCustom")]
        //public bool IsCustom { get; set; }

        //[XmlElement("IsOpen")]
        //public bool IsOpen { get; set; }

        //[XmlElement("Layout")]
        //public int Layout { get; set; }

        //[XmlElement("Local")]
        //public bool Local { get; set; }

        [XmlElement("Path")]
        public string Path { get; set; }
    }
}