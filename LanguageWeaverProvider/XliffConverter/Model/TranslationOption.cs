using System.Xml.Serialization;

namespace LanguageWeaverProvider.XliffConverter.Model
{
	public class TranslationOption
	{	
		[XmlAttribute("tool-id")]
		public string ToolId { get; set; }

		[XmlAttribute("date")]
		public string Date { get; set; }

		[XmlElement("target")]
		public TargetTranslation Translation { get; set; }
		
		[XmlAttribute("match-quality")]
		public string MatchQuality { get; set; }
	}
}