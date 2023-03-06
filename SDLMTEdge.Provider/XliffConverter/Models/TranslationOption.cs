using System.Xml.Serialization;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Models
{
	public class TranslationOption
	{	
		[XmlAttribute("tool-id")]
		public string ToolId { get; set; }

		[XmlAttribute("date")]
		public string Date { get; set; }

		[XmlElement("target")]
		public TargetTranslation Translation { get; set; }
	}
}