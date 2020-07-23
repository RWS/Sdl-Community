using System.Xml.Serialization;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
	/// <summary>
	/// For deserializing XML files containing lists for batch find + replace in source text before MT and in target text after MT 
	/// </summary>
	public class EditItem
	{
		public enum EditItemType
		{
			[XmlEnum(Name = "plain_text")]
			PlainText,
			[XmlEnum(Name = "regular_expression")]
			RegularExpression
		};
		public string FindText { get; set; }
		public string ReplaceText { get; set; }

		[XmlAttribute(AttributeName = "EditItemType")]
		public EditItemType Type { get; set; }

		[XmlAttribute(AttributeName = "Enabled")]
		public bool Enabled;
	}
}