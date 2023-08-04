using System.Xml.Serialization;

namespace LanguageWeaverProvider.XliffConverter.Models
{
	public class Header
	{
		[XmlElement("tool")]
		public Tool[] Tools { get; set; }		
	}
}
