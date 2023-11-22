using System;
using Multilingual.Excel.FileType.Common;

namespace Multilingual.Excel.FileType.Models
{
	public class CommentProperty
	{
		public string Name { get; set; }

		public Enumerators.CommentPropertyType Type { get; set; }

		public Type ValueType { get; set; }

		public string Description { get; set; }
	}
}
