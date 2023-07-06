using System.Xml.Serialization;

namespace GoogleCloudTranslationProvider
{
	public enum ApiVersion
	{
		V3,
		V2
	}

	public enum EditItemType
	{
		[XmlEnum(Name = "plain_text")]
		PlainText,
		[XmlEnum(Name = "regular_expression")]
		RegularExpression
	};

	public enum Parameters
	{
		Inverted
	}
}