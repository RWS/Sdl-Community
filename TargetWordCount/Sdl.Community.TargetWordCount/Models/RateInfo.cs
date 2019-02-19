using System.ComponentModel;
using System.Xml.Serialization;

namespace Sdl.Community.TargetWordCount.Models
{
	public class RateInfo
    {
        [XmlArray("Invoice")]
        public BindingList<InvoiceItem> Rate { get; set; } = new BindingList<InvoiceItem>();
    }
}