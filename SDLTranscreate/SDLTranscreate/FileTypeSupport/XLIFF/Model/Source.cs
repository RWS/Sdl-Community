using System;
using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class Source : Segment, ICloneable
	{
		public Source()
		{
			Elements = new List<Element>();			
		}

		public string Id { get; set; }

		public object Clone()
		{
			var source = new Source();
			source.Id = Id;
			foreach (var element in Elements)
			{
				source.Elements.Add(element.Clone() as Element);
			}

			return source;
		}
	}
}
