using System.Collections.Generic;
using System.Xml.Serialization;

namespace LanguageWeaverProvider.XliffConverter.Model
{
	public class Body
	{
		public Body()
		{
			TranslationUnits = new List<TranslationUnit>();
		}

		[XmlElement("trans-unit")]
		public List<TranslationUnit> TranslationUnits { get; set; }
	}
}