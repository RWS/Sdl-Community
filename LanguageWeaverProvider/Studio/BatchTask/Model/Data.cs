using System.Collections.Generic;
using System.Xml.Serialization;

namespace LanguageWeaverProvider.Studio.BatchTask.Model
{
	public class Data
	{
		[XmlElement]
		public List<File> File { get; set; } = new();
	}
}