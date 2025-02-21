using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public abstract class Segment
	{
		public List<Element> Elements { get; set; }

		public override string ToString()
		{
			var content = string.Empty;

			foreach (var element in Elements)
			{
				if (element is ElementComment)
				{
					continue;
				}

				if (element is ElementText text)
				{
					content += text.Text;
				}

				if (element is ElementPlaceholder placeholder)
				{
					content += placeholder.TagContent;
				}

				if (element is ElementTagPair tag)
				{
					content += tag.TagContent;
				}
			}
			return content;
		}
	}
}
