using System;
using Multilingual.Excel.FileType.Models;

namespace Multilingual.Excel.FileType.Providers.OpenXml.Model
{
	public class ExcelCell: ICloneable
	{
		public string Value { get; set; }

		public Hyperlink Hyperlink { get; set; }

		public ExcelColumn Column { get; set; }

		public string Background { get; set; }
		
		public object Clone()
		{
			return new ExcelCell
			{
				Background = Background,
				Value = Value,
				Hyperlink = Hyperlink,
				Column = Column.Clone() as ExcelColumn
			};
		}
	}
}
