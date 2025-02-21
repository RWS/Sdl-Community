using System;
using Multilingual.XML.FileType.Common;

namespace Multilingual.XML.FileType.Models
{
	public class CommentProperty
	{
		public string Name { get; set; }

		public Enumerators.CommentPropertyType Type { get; set; }

		public Type ValueType { get; set; }

		public string Description { get; set; }
	}
}
