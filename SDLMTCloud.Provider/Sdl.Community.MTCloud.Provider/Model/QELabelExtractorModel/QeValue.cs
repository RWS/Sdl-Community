using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class QeValue
	{
		[XmlAttribute]
		public int SegmentsTotal { get; set; }

		[XmlAttribute]
		public int WordsTotal { get; set; }

		[XmlText]
		public string QualityEstimation{ get; set; }
	}
}