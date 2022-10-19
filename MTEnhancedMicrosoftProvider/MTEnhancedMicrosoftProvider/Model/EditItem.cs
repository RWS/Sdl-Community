using System.Xml.Serialization;

namespace MicrosoftTranslatorProvider.Model
{
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
		public bool Enabled { get; set; }
	}
}