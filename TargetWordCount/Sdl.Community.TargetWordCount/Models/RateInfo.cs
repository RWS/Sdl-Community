using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.Community.TargetWordCount.Models
{
	public class RateInfo
    {
        [XmlArray("Invoice")]
        public List<InvoiceItem> Rate { get; set; } = new List<InvoiceItem>();
    }
}