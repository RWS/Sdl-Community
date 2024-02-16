using System.Collections.Generic;
using System.Xml.Serialization;

namespace LanguageWeaverProvider.Studio.BatchTask.Model
{
	public class File
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlElement]
		public IEnumerable<QeValue> QeValues { get; set; }
	}
}