using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.XliffConverter.Models
{
	public class TranslationOption
	{
		[XmlAttribute("date")]
		public string Date { get; set; }

		[XmlAttribute("match-quality")]
		public string MatchQuality { get; set; }

		[XmlAttribute("tool-id")]
		public string ToolId { get; set; }

		[XmlElement("target")]
		public TargetTranslation Translation { get; set; }
	}
}