namespace Multilingual.XML.FileType.Models
{
	public class ElementTagPair: Element
	{			
		public TagType Type { get; set; }

		public string TagId { get; set; }

		public string TagContent { get; set; }

		public string DisplayText { get; set; }
	}
}
