using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class QeValue
	{
		[XmlAttribute]
		public int Total { get; set; }

		[XmlText]
		public string QualityEstimation{ get; set; }
	}
}