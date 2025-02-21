using System;

namespace Multilingual.XML.FileType.Models
{
	public class CommentPropertyMapping: ICloneable
	{
		public string StudioPropertyName { get; set; }

		public string PropertyName { get; set; }

		public string PropertyType { get; set; }

		public object Clone()
		{
			return new CommentPropertyMapping
			{
				StudioPropertyName = StudioPropertyName,
				PropertyName = PropertyName,
				PropertyType = PropertyType
			};
		}
	}
}
