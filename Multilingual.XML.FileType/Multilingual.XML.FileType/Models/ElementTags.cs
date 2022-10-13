using System.Collections.Generic;

namespace Multilingual.XML.FileType.Models
{
	public class ElementTags
	{
		public ElementTags()
		{
			TagPairElements = new List<ElementTagPair>();
			PlaceholderElements = new List<ElementPlaceholder>();
		}
		
		public List<ElementTagPair> TagPairElements { get; set; }

		public List<ElementPlaceholder> PlaceholderElements { get; set; }
	}
}
