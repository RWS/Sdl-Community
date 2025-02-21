using System.Xml.Serialization;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
    public enum EditItemType
    {
        [XmlEnum(Name = "plain_text")]
        PlainText,
        [XmlEnum(Name = "regular_expression")]
        RegularExpression
    };
}