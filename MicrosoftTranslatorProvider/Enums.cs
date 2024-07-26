using System.Xml.Serialization;

namespace MicrosoftTranslatorProvider
{
	public enum Parameters
	{
		Inverted
	}

	public enum EditItemType
	{
		[XmlEnum(Name = "plain_text")]
		PlainText,
		[XmlEnum(Name = "regular_expression")]
		RegularExpression
	};
}
