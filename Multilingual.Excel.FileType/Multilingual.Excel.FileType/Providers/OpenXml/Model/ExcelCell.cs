using System;

namespace Multilingual.Excel.FileType.Providers.OpenXml.Model
{
	public class ExcelCell: ICloneable
	{
		public string Value { get; set; }

		public ExcelColumn Column { get; set; }
		
		public object Clone()
		{
			return new ExcelCell
			{
				Value = Value,
				Column = Column.Clone() as ExcelColumn
			};

		}
	}
}
