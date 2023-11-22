using System;
using System.Collections.Generic;

namespace Multilingual.XML.FileType.Models
{
	public class ParagraphUnitContext : ICloneable
	{
		public ParagraphUnitContext()
		{
			MetaData = new Dictionary<string, string>();
		}

		public string Id { get; set; }

		public string ContextType { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public string DisplayCode { get; set; }

		public Dictionary<string, string> MetaData { get; set; }

		public object Clone()
		{
			var context = new ParagraphUnitContext
			{
				Id = Id,
				ContextType = ContextType,
				DisplayName = DisplayName,
				Description = Description,
				DisplayCode = DisplayCode
			};

			foreach (var item in MetaData)
			{
				context.MetaData.Add(item.Key, item.Value);
			}

			return context;
		}
	}
}
