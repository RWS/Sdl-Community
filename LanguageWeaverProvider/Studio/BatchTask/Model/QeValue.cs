using System.Xml.Serialization;

namespace LanguageWeaverProvider.Studio.BatchTask.Model
{
	public class QeValue
	{
		[XmlAttribute]
		public int SegmentsTotal { get; set; }

		[XmlAttribute]
		public int WordsTotal { get; set; }

		[XmlAttribute]
		public int CharactersTotal { get; set; }

		[XmlText]
		public string QualityEstimation { get; set; }
	}
}