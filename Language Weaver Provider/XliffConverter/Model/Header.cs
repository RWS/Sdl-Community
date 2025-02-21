using System.Xml.Serialization;

namespace LanguageWeaverProvider.XliffConverter.Model
{
	public class Header
	{
		[XmlElement("tool")]
		public Tool[] Tools { get; set; }		
	}
}