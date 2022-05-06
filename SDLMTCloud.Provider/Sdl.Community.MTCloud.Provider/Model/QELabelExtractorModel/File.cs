using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class File
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlElement]
		public List<QeValue> QeValues { get; set; } = new();
	}
}