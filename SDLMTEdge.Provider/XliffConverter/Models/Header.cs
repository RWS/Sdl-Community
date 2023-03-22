using System.Xml.Serialization;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Models
{
	public class Header
	{
		[XmlElement("tool")]
		public Tool[] Tools { get; set; }		
	}
}