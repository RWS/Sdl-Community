using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class QeValue
	{
		[XmlText]
		public string QualityEstimation { get; set; }

		[XmlAttribute]
		public int SegmentsTotal { get; set; }

		[XmlAttribute]
		public int WordsTotal { get; set; }
	}
}