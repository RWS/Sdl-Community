using System.Xml.Serialization;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Model
{
    /// <summary>
    /// For deserializing XML files containing lists for batch find + replace in source text before MT and in target text after MT 
    /// </summary>
    public class EditItem
    {
        public string FindText { get; set; }
        public string ReplaceText { get; set; }

        [XmlAttribute(AttributeName = "EditItemType")]
        public EditItemType Type { get; set; }

        [XmlAttribute(AttributeName = "Enabled")]
        public bool Enabled;
    }
}