using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.XliffConverter.Models
{
	public class Header
	{
		[XmlElement("tool")]
		public Tool[] Tools { get; set; }		
	}
}
