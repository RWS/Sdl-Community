using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.Community.CleanUpTasks.Models
{
	[XmlRoot("ConversionItems")]
    public class ConversionItemList
    {
        [XmlArray("Items"), XmlArrayItem(typeof(ConversionItem), ElementName = "Item")]
        public List<ConversionItem> Items { get; set; } = new List<ConversionItem>();
    }
}