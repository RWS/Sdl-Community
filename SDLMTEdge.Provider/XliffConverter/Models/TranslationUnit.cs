using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Models
{
	public class TranslationUnit
	{
		[XmlAttribute("id")]
		public int Id { get; set; }

		[XmlElement("source")]
		public string SourceText { get; set; }

		[XmlElement("alt-trans")]
		public List<TranslationOption> TranslationList { get; set; }
	}
}