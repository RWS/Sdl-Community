using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.XliffConverter.Models
{
	public class Tool
	{
		[XmlAttribute("tool-id")]
		public string ToolId { get; set; }

		[XmlAttribute("tool-name")]
		public string ToolName { get; set; }

		[XmlAttribute("tool-version")]
		public string ToolVersion { get; set; }
	}
}