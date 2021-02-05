using System;
using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class Target : Segment, ICloneable
	{
		public Target()
		{
			Elements = new List<Element>();
		}

		public string Id { get; set; }

		public object Clone()
		{
			var target = new Target();
			target.Id = Id;
			foreach (var element in Elements)
			{
				target.Elements.Add(element.Clone() as Element);
			}

			return target;
		}
	}
}
