using System.Collections.Generic;
using System.Xml.Serialization;

namespace LanguageWeaverProvider.Studio.BatchTask.Model
{
	public class File
	{
		public File(string fileName)
		{
			Name = fileName.Trim();
		}

		[XmlAttribute]
		public string Name { get; set; }

		[XmlElement]
		public List<QeValue> QeValues { get; set; } = new();
	}
}