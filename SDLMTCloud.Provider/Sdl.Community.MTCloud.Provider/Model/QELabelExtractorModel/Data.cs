using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class Data
	{
		[XmlElement]
		public List<File> File { get; set; } = new();
	}
}